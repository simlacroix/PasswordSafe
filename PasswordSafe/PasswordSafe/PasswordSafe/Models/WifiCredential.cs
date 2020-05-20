using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class WifiCredential : Credential
    {
        private string _wifiName;
        public string WifiName
        {
            get { return _wifiName; }
            set
            {
                if (_wifiName == value)
                    return;

                _wifiName = value;
                OnPropertyChanged(nameof(WifiName));
            }
        }

        private string _macAddress;
        public string MacAddress
        {
            get { return _macAddress; }
            set
            {
                if (_macAddress == value)
                    return;

                _macAddress = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                if (_ipAddress == value)
                    return;

                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        private string _subnetMask;
        public string SubnetMask
        {
            get { return _subnetMask; }
            set
            {
                if (_subnetMask == value)
                    return;

                _subnetMask = value;
                OnPropertyChanged(nameof(SubnetMask));
            }
        }

        private string _defaultGateway;
        public string DefaultGateway
        {
            get { return _defaultGateway; }
            set
            {
                if (_defaultGateway == value)
                    return;

                _defaultGateway = value;
                OnPropertyChanged(nameof(DefaultGateway));
            }
        }

        private string _dnsServer;
        public string DnsServer
        {
            get { return _dnsServer; }
            set
            {
                if (_dnsServer == value)
                    return;

                _dnsServer = value;
                OnPropertyChanged(nameof(DnsServer));
            }
        }
    }
}
