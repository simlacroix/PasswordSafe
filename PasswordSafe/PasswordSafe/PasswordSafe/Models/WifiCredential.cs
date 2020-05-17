using System;
using System.Collections.Generic;
using System.Text;

namespace PasswordSafe.Models
{
    public class WifiCredential : Credential
    {
        public string WifiName { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string DefaultGateway { get; set; }
        public string DnsServer { get; set; }
    }
}
