using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class SocialMediaCredential : Credential
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
    }
}
