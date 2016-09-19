using NCM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NCM.Database
{
    public class Configuration
    {
        public int ConfigurationId { get; set; }
        /// <summary>
        /// Alternatively used as SSID
        /// </summary>
        public string Name { get; set; }
        public ConfigurationType ConfigurationType { get; set; }
        public bool IsActive { get; set; }
        public string IP { get; set; }
        public string SubnetMask { get; set; }
        public string Gateway { get; set; }
        public List<string> DNSs { get; set; }
    }
}
