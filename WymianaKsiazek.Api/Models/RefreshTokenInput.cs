using System.ComponentModel.DataAnnotations;

namespace WymianaKsiazek.Api.Models
{
    public class RefreshTokenInput
    {
        [Required]
        public string Token { get; set; }
    }
}
