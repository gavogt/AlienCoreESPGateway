using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class RegistrationModel
    {
        [Required]
        public string Email { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;
        [Required]
        public string FirstName { get; set; } = String.Empty;
        [Required]
        public string LastName { get; set; } = String.Empty;
        [Required]
        public string ConfirmPassword { get; set; } = String.Empty;


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Password) &&
                   Password == ConfirmPassword;
        }
    }
}
