namespace PharmaNet.Client.Models
{
    public class JwtResponse
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }
}