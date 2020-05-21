using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PasswordSafe.DataHelper
{
    public class IOPassOperations
    {
        public string FileName { get; set; }

        public IOPassOperations(string passPath)
        {
            FileName = passPath;
        }

        // hashing verify method method found here: https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129
        public bool VerifyPass(string attempt)
        {           
            string storedCombination = File.ReadAllText(FileName);
            byte[] hashBytes = Convert.FromBase64String(storedCombination);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var derivedBytes = new Rfc2898DeriveBytes(attempt, salt, 10000);
            byte[] hash = derivedBytes.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        public bool CheckPass()
        {
            if (!File.Exists(FileName))
                return false;
            else if (String.IsNullOrWhiteSpace(File.ReadAllText(FileName)))
                return false;
            else
                return true;
        }

        public bool ChangePass(string pass)
        {
            if (pass.Length < 6 || !Regex.IsMatch(pass, "[A-Z]") || !Regex.IsMatch(pass, "[a-z]") || !Regex.IsMatch(pass, "[+,-,=,_,@,#,$,%,^,&,*,(,),{,},;,:,\',?,!]") || !Regex.IsMatch(pass, "[0-9]"))
                return false;

            File.WriteAllText(FileName, Hash(pass));
            return true;
        }

        // hashing method found here: https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129 
        private string Hash(string input)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var derivedBytes = new Rfc2898DeriveBytes(input, salt, 10000);
            byte[] hash = derivedBytes.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedCombinationHash = Convert.ToBase64String(hashBytes);
            return savedCombinationHash;
        }        
        
    }
}
