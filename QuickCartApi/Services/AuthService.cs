using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuickCartApi.Common;
using QuickCartApi.DTOs.Auth;
using QuickCartApi.DTOs.Users;
using QuickCartApi.Entities;
using QuickCartApi.Enums;
using QuickCartApi.Options;
using QuickCartApi.Repositories.Interfaces;
using QuickCartApi.Services.Interfaces;

namespace QuickCartApi.Services
{
    public class AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IOptions<JwtOptions> jwtOptions,
            IHostEnvironment environment
        ) : IAuthService
    {
        private const string REFRESH_COOKIE_NAME = "refresh_token";
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private static readonly TimeSpan ReuseGracePeriod = TimeSpan.FromSeconds(30);

        public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest request)
        {
            var username = request.Username.Trim();
            var email = request.Email.Trim();

            var usernameExists = await userRepository.FindUserNameExistAsync(username);

            var emailExists = await userRepository.FindEmailExistAsync(email);

            if (usernameExists || emailExists)
            {
                var errors = new Dictionary<string, string[]>();

                if (usernameExists)
                    errors["username"] = ["Username already exists."];

                if (emailExists)
                    errors["email"] = ["Email already exists."];

                return Result.Fail<UserResponse>(
                    ErrorKind.Conflict,
                    "Registration failed.",
                    errors
                );
            }

            User user = new()
            {
                Username = username,
                Role = UserRole.Customer.ToString(),
                FullName = request.FullName,
                Email = email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            User created;

            try
            {
                created = await userRepository.AddUserAsync(user);
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 2601 or 2627 })
            {   
                // unique index violated: someone registered the same username or email a moment ago
                return Result.Fail<UserResponse>(ErrorKind.Conflict, "Username or email already exists.");
            }

            var response = new UserResponse
            {
                Id = created.Id,
                Username = created.Username,
                Role = created.Role,
                FullName = created.FullName,
                Email = created.Email,
                Phone = created.Phone,
                CreatedAt = created.CreatedAt
            };

            return Result.Ok(response);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request, HttpResponse response)
        {
            var user = await userRepository.FindUserAsync(request.Username);

            if (user is null)
                return null;

            // check the hash and submitted password from the request to verify of its correct
            var verify = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verify == PasswordVerificationResult.Failed)
                return null;

            return await IssueTokensAsync(user, response);
        }

        public async Task<AuthResponse?> RefreshAsync(HttpRequest request, HttpResponse response)
        {
            if (!request.Cookies.TryGetValue(REFRESH_COOKIE_NAME, out var rawToken) || string.IsNullOrWhiteSpace(rawToken))
                return null;

            var tokenHash = tokenService.HashToken(rawToken);
            var existingToken = await refreshTokenRepository.FindByHashAsync(tokenHash);

            if (existingToken is null)
            {
                DeleteRefreshCookie(response);   // unknown token: stop the browser from sending it again
                return null;
            }

            // reuse detection a revoked token being used again means it is stollen
            // revoke all the session of that user
            if (existingToken.RevokedAtUtc is not null && DateTime.UtcNow - existingToken.RevokedAtUtc.Value > ReuseGracePeriod)
            {
                await refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId);
                DeleteRefreshCookie(response);
                return null;
            }

            if (existingToken.ExpiresAtUtc <= DateTime.UtcNow)
            {
                DeleteRefreshCookie(response);   // expired: clear the dead cookie
                return null;
            }

            // inside the grace period the token is already revoked, so don't revoke it twice
            if (existingToken.RevokedAtUtc is null)
                await refreshTokenRepository.RevokeAsync(existingToken);

            var user = existingToken.User ?? await userRepository.FindUserByIdAsync(existingToken.UserId);
            if (user is null)
                return null;

            return await IssueTokensAsync(user, response);
        }

        public async Task<UserResponse?> GetCurrentUserAsync(int userId)
        {
            var user = await userRepository.FindUserByIdAsync(userId);
            if (user is null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task LogoutAsync(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies.TryGetValue(REFRESH_COOKIE_NAME, out var rawToken) && !string.IsNullOrWhiteSpace(rawToken))
            {
                var tokenHash = tokenService.HashToken(rawToken);
                var existingToken = await refreshTokenRepository.FindByHashAsync(tokenHash);

                if (existingToken is not null && existingToken.RevokedAtUtc is null)
                    await refreshTokenRepository.RevokeAsync(existingToken);
            }

            DeleteRefreshCookie(response);
        }

        private async Task<AuthResponse> IssueTokensAsync(User user, HttpResponse response)
        {
            var accessToken = tokenService.CreateAccessToken(user);
            var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

            var rawRefreshToken = tokenService.CreateRefreshToken();
            var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

            await refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = tokenService.HashToken(rawRefreshToken),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = refreshTokenExpiresAtUtc
            });

            SetRefreshCookie(response, rawRefreshToken, refreshTokenExpiresAtUtc);

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc
            };
        }

        private CookieOptions RefreshCookieOptions(DateTimeOffset? expires = null)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,   // frontend and API are on different sites in prod
                Path = "/api/v1/auth",
                Expires = expires
            };

            // lets the cookie survive third-party cookie blocking by keying it to the frontend's site.
            // production only, so local development keeps behaving exactly as it does now.
            if (!environment.IsDevelopment())
                options.Extensions.Add("Partitioned");

            return options;
        }

        private void SetRefreshCookie(HttpResponse response, string token, DateTime expiresAtUtc)
            => response.Cookies.Append(REFRESH_COOKIE_NAME, token, RefreshCookieOptions(expiresAtUtc));

        private void DeleteRefreshCookie(HttpResponse response)
            => response.Cookies.Append(REFRESH_COOKIE_NAME, string.Empty, RefreshCookieOptions(DateTimeOffset.UnixEpoch));
    }
}
