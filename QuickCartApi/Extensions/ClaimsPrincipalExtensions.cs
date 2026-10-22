using System.Security.Claims;

namespace QuickCartApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(value, out var id) ? id : throw new InvalidOperationException("The token has no valid user id claim.");
        }
    }
}
