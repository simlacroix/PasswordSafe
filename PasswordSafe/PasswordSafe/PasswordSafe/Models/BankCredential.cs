using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class BankCredential : Credential
    {
        public long CardNumber { get; set; }
        public int SecuirityCode { get; set; }

        private Dictionary<string, string> securityQuestions = new Dictionary<string, string>();
        public void AddSecurityQA(string question, string answer) {
            securityQuestions.Add(question,answer);
        }
        public Dictionary<string, string> GetSecurityQuestions() {
            return securityQuestions;
        }

        public string Address { get; set; }
        public string onlineBankingUrl { get; set; }

        private List<long> accounts = new List<long>();
        public List<long> GetAccounts() {
            return accounts;
        }
        public void AddAccounts(long newAccount) {
            accounts.Add(newAccount);
        }

    }
}
