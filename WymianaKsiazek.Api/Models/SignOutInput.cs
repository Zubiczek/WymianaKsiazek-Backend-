using System.ComponentModel.DataAnnotations;

namespace WymianaKsiazek.Api.Models
{
    public class SignOutInput
    {
        [Required]
        public string Token { get; set; }
    }
}
