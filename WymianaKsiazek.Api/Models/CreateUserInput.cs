
using System.ComponentModel.DataAnnotations;

namespace WymianaKsiazek.Api
{
    public class CreateUserInput
    {
        public string Name { get; set; }

        [Required(ErrorMessage = "Adres e-mail wymagany")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło wymagane")]
        public string Password { get; set; }
    }
}
