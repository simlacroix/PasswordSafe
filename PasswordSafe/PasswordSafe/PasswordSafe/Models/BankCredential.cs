using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class BankCredential : Credential
    {
        public long CardNumber { get; set; }
        public int SecuirityCode { get; set; }
        public string Address { get; set; }
        public string OnlineBankingUrl { get; set; }

        public List<long> Accounts { get; }
        public Dictionary<string, string> SecurityQuestions { get; }

        // do we need this?
        public void AddAccounts(long newAccount) {
            Accounts.Add(newAccount);
        }
        public void AddSecurityQA(string question, string answer)
        {
            SecurityQuestions.Add(question, answer);
        }
    }
}
