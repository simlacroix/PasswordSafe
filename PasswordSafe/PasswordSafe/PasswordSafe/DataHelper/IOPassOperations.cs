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

        public void ChangePass(string pass)
        {

        }

        public bool VerifyPass(string attempt)
        {

            //placeholder
            return false;
        }

        public bool CheckPass()
        {

            //placeholder
            return false;
        }

    }
}
