namespace IdentityShield.Contracts.Models
{
    public class JwtResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
