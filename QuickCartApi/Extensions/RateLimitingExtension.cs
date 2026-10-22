using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace QuickCartApi.Extensions
{
    public static class RateLimitingExtension
    {
        public const string AuthPolicy = "auth"; // login, register
        public const string RefreshPolicy = "refresh"; // token refresh
        public const string WritePolicy = "write"; // crud
        public const string ReadPolicy = "read"; // public reads

        public static IServiceCollection AddRateLimitingSetup(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    var response = context.HttpContext.Response;
                    response.ContentType = "application/json";

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        response.Headers.RetryAfter = ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString();

                    var payload = JsonSerializer.Serialize(new
                    {
                        message = "Too many requests. Please try again later."
                    });

                    await response.WriteAsync(payload, token);
                };

                AddFixedWindowPolicy(options, AuthPolicy, permitLimit: 5);
                AddFixedWindowPolicy(options, RefreshPolicy, permitLimit: 20);
                AddFixedWindowPolicy(options, WritePolicy, permitLimit: 30);
                AddFixedWindowPolicy(options, ReadPolicy, permitLimit: 120, queueLimit: 10);
            });

            return services;
        }

        private static void AddFixedWindowPolicy(RateLimiterOptions options, string policyName, int permitLimit, int queueLimit = 0)
        {
            options.AddPolicy(policyName, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: GetClientKey(httpContext),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = queueLimit
                    }));
        }

        // authenticated requests are limited per user, anonymous ones per IP
        private static string GetClientKey(HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return !string.IsNullOrEmpty(userId)
                ? $"user:{userId}"
                : $"ip:{httpContext.Connection.RemoteIpAddress}";
        }
    }
}
