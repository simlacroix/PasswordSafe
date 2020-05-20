using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class SocialMediaCredential : Credential
    {
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email == value)
                    return;

                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                    return;

                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber == value)
                    return;

                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set
            {
                if (_link == value)
                    return;

                _link = value;
                OnPropertyChanged(nameof(Link));
            }
        }
    }
}
