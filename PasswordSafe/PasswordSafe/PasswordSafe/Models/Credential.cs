using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class Credential
    {
        public int ID { get; set; }
        public string CredentialTitle { get; set; }
        public string Password { get; set; }
        public DateTime expirationDate { get; set; }
        public string Notes { get; set; }
        public PasswordPolicy passwordPolicy { get; set; }

        protected List<string> passwordHistory = new List<string>();
        public void UpdatePasswordPolicy(string oldPassword) {
            passwordHistory.Add(oldPassword);
        }
        public string GetMostRecentPassword() {
            if(passwordHistory.Count!=0)
                return passwordHistory[-1];
            return "";
        }
    }

    public class BaseCredential : Credential { 
    
    }
}
