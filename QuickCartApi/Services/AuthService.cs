using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using QuickCartApi.DTOs.Auth;
using QuickCartApi.DTOs.Users;
using QuickCartApi.Enums;
using QuickCartApi.Entities;
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

        public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
        {
            // check if the username already exists in the db
            if (await userRepository.FindUserNameExistAsync(request.Username))
                return null;

            User user = new()
            {
                Username = request.Username,
                Role = UserRole.Customer.ToString(),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            var created = await userRepository.AddUserAsync(user);

            return new UserResponse
            {
                Id = created.Id,
                Username = created.Username,
                Role = created.Role,
                FullName = created.FullName,
                Email = created.Email,
                Phone = created.Phone,
                CreatedAt = created.CreatedAt
            }; 
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
                return null;

            // reuse detection a revoked token being used again means it is stollen
            // revoke all the session of that user
            if (existingToken.RevokedAtUtc is not null)
            {
                await refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId);
                DeleteRefreshCookie(response);
                return null;
            }

            if (existingToken.ExpiresAtUtc <= DateTime.UtcNow)
                return null;

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

        private void SetRefreshCookie(HttpResponse response, string token, DateTime expiresAtUtc)
        {
            response.Cookies.Append(REFRESH_COOKIE_NAME, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = !environment.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Expires = expiresAtUtc,
                Path = "/api/auth"
            });
        }

        private void DeleteRefreshCookie(HttpResponse response)
        {
            response.Cookies.Delete(REFRESH_COOKIE_NAME, new CookieOptions
            {
                Path = "/api/auth"
            });
        }
    }
}
