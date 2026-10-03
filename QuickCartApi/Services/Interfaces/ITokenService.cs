using QuickCartApi.Entities;

namespace QuickCartApi.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
        string HashToken(string token);
    }
}
