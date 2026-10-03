using QuickCartApi.Entities;

namespace QuickCartApi.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> AddAsync(RefreshToken token);
        Task<RefreshToken?> FindByHashAsync(string tokenHash);
        Task RevokeAsync(RefreshToken token);
        Task RevokeAllForUserAsync(int userId);
    }
}
