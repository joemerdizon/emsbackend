namespace Entities.Models
{
    public class JwtToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public double SessionTimeout { get; set; }
    }
}
