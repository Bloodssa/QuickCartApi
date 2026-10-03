using Microsoft.EntityFrameworkCore;
using QuickCartApi.Data;
using QuickCartApi.Entities;
using QuickCartApi.Repositories.Interfaces;

namespace QuickCartApi.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken> AddAsync(RefreshToken token)
        {
            await context.RefreshTokens.AddAsync(token);
            await context.SaveChangesAsync();

            return token;
        }

        public async Task<RefreshToken?> FindByHashAsync(string tokenHash)
        {
            return await context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }
        public async Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }

        public async Task RevokeAllForUserAsync(int userId)
        {

            // get the tokens of the users that is not expired
            var activeTokens = await context.RefreshTokens
                    .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
                    .ToListAsync();

            foreach (var token in activeTokens)
                token.RevokedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }

    }
}
