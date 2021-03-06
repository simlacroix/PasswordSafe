﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PasswordSafe.Models
{
    public class BankCredential : Credential
    {
        private long _cardNumber;
        public long CardNumber
        {
            get { return _cardNumber; }
            set
            {
                if (_cardNumber == value)
                    return;

                _cardNumber = value;
                OnPropertyChanged(nameof(CardNumber));
            }
        }

        private int _securityCode;
        public int SecurityCode
        {
            get { return _securityCode; }
            set
            {
                if (_securityCode == value)
                    return;

                _securityCode = value;
                OnPropertyChanged(nameof(SecurityCode));
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                if (_address == value)
                    return;

                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string _onlineBankingUrl;
        public string OnlineBankingUrl
        {
            get { return _onlineBankingUrl; }
            set
            {
                if (_onlineBankingUrl == value)
                    return;

                _onlineBankingUrl = value;
                OnPropertyChanged(nameof(OnlineBankingUrl));
            }
        }

        private string _accounts;
        public string Accounts
        {
            get { return _accounts; }
            set
            {
                if (_accounts == value)
                    return;

                _accounts = value;
                OnPropertyChanged(nameof(Accounts));
            }
        }

        private string _securityQuestions;
        public string SecurityQuestions
        {
            get { return _securityQuestions; }
            set
            {
                if (_securityQuestions == value)
                    return;

                _securityQuestions = value;
                OnPropertyChanged(nameof(SecurityQuestions));
            }
        }
    }
}
