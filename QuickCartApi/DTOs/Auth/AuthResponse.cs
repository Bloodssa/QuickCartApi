namespace QuickCartApi.DTOs.Auth
{
    /**
     * Auth response for the login and refresh token of the signed jwt
     */
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
    }
}
    