using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlienCoreESPGateway
{
    public class User
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public User(string Email, string Password, string FirstName, string LastName)
        {
            this.Email = Email;
            this.Password = Password;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }
    }
}
