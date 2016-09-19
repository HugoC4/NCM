using NCM.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NCM.Database
{
    public class Adapter
    {
        public int AdapterId { get; set; }
        public PhysicalAddress MacAddress { get; set; }
        public AdapterType AdapterType { get; set; }
        public virtual List<Configuration> Configurations { get; set; }
    }
}
