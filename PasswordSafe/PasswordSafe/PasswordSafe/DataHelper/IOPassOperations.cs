using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PasswordSafe.DataHelper
{
    public class IOPassOperations
    {
        public string FileName { get; set; }

        public IOPassOperations(string passPath)
        {
            FileName = passPath;
        }
        
        public bool VerifyPass(string attempt)
        {
            //will be more complicated later because of encryption
            if (File.ReadAllText(FileName) == attempt)
                return true;
            else
                return false;
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

        public void ChangePass(string pass)
        {
            //need to do some encryption here
            File.WriteAllText(FileName, pass);
        }

    }
}
