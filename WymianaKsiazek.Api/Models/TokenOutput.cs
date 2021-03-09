using System;

namespace WymianaKsiazek.Api.Models
{
    public class TokenOutput
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
