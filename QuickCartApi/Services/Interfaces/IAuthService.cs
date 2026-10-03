using QuickCartApi.DTOs.Auth;
using QuickCartApi.DTOs.Users;

namespace QuickCartApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse?> RegisterAsync(RegisterRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request, HttpResponse response);
        Task<AuthResponse?> RefreshAsync(HttpRequest request, HttpResponse response);
        Task<UserResponse?> GetCurrentUserAsync(int userId);
        Task LogoutAsync(HttpRequest request, HttpResponse response);
    }
}
