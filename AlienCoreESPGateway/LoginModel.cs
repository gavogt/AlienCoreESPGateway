using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; } = String.Empty;
        [Required]
        public string Password { get; set; } = String.Empty;

    }
}
