using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PasswordSafe.Models
{
    public class Credential:INotifyPropertyChanged
    {
        public static string[] credentialTypes = new string[] {"Default","Social Media","Banking","Wifi"};

        // AutoIncrement: no need to add manually if user doesn't provide it
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        private string _credentialTitle;
        public string CredentialTitle
        {
            get { return _credentialTitle; }
            set
            {
                if (_credentialTitle == value)
                    return;

                _credentialTitle = value;
                OnPropertyChanged(nameof(CredentialTitle));
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value)
                    return;

                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private DateTime _expirationDate;
        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set
            {
                if (_expirationDate == value)
                    return;

                _expirationDate = value;
                OnPropertyChanged(nameof(ExpirationDate));
            }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes == value)
                    return;

                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        private List<string> passwordHistory = new List<string>();

        public string GetMostRecentPassword() {
            if(passwordHistory.Count!=0)
                return passwordHistory[-1];
            return "";
        }

        public List<string> GetPasswordHistory() {
            return passwordHistory;
        }

        public bool IsChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {

            if (PropertyChanged != null)
            {
                IsChanged = true;
                // this property is changed, raise an property changed event
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
