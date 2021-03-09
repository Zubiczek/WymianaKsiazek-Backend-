using System;
using System.ComponentModel.DataAnnotations;

namespace WymianaKsiazek.Api.Models
{
    public class CreateTokenInput
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
