using NativeWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NCM.Networks
{
    class NetworkManager
    {
        ManagementClass Manager { get; set; }
        WlanClient WlanClient { get; set; }
        Dictionary<PhysicalAddress, ManagementObject> Adapters {get;set;}
        
        public NetworkManager()
        {
            Manager = new ManagementClass("Win32_NetworkAdapter");
            WlanClient = new WlanClient();
            Refresh();
        }

        public void Refresh()
        {
            using (var networkCollection = Manager.GetInstances())
            {
                foreach (ManagementObject adapter in networkCollection)
                {
                    if (adapter.Properties ==null || string.IsNullOrWhiteSpace((string)adapter["MACAddress"]) || string.IsNullOrWhiteSpace((string)adapter["NetConnectionID"]))
                        continue;

                    PhysicalAddress address;

                    try
                    {
                        address = PhysicalAddress.Parse((string)adapter["MACAddress"]);

                    }
                    catch (Exception e) {
                        continue;
                    }

                    Adapters[address] = adapter;
                }
            }
        }

        public ManagementObject GetAdapter(PhysicalAddress adapterKey)
        {
            if (!Adapters.ContainsKey(adapterKey))
                Refresh();
            return Adapters[adapterKey] ?? null;
        }

        /// <summary>
        /// Set's a new IP Address and it's Submask of the local machine
        /// </summary>
        /// <param name="ip_address">The IP Address</param>
        /// <param name="subnet_mask">The Submask IP Address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void setStaticIP(PhysicalAddress adapterKey, string ip_address, string subnet_mask)
        {
            ManagementObject objMO = GetAdapter(adapterKey);            
            try
            {
                ManagementBaseObject setIP;
                ManagementBaseObject newIP =
                    objMO.GetMethodParameters("EnableStatic");

                newIP["IPAddress"] = new string[] { ip_address };
                newIP["SubnetMask"] = new string[] { subnet_mask };

                setIP = objMO.InvokeMethod("EnableStatic", newIP, null);
            }
            catch (Exception)
            {             }                      
        }

        public void setDynamicIP(PhysicalAddress adapterKey)
        {
            ManagementObject mo = GetAdapter(adapterKey);
            try
            {
                ManagementBaseObject ndns = mo.GetMethodParameters("SetDNSServerSearchOrder");
                ndns["DNSServerSearchOrder"] = null;
                ManagementBaseObject enableDhcp = mo.InvokeMethod("EnableDHCP", null, null);
                ManagementBaseObject setDns = mo.InvokeMethod("SetDNSServerSearchOrder", ndns, null);
            }
            catch (Exception e) { }
        }
        /// <summary>
        /// Set's a new Gateway address of the local machine
        /// </summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void setGateway(PhysicalAddress adapterKey, string gateway)
        {
            var objMO = GetAdapter(adapterKey);
                    try
                    {
                        ManagementBaseObject setGateway;
                        ManagementBaseObject newGateway =
                            objMO.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = new string[] { gateway };
                        newGateway["GatewayCostMetric"] = new int[] { 1 };

                        setGateway = objMO.InvokeMethod("SetGateways", newGateway, null);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
        }
        /// <summary>
        /// Set's the DNS Server of the local machine
        /// </summary>
        /// <param name="nic">NIC address</param>
        /// <param name="dnsServers">Comma seperated list of DNS server addresses</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void SetDNS(PhysicalAddress adapterKey, string nicDescription, string[] dnsServers)
        {
            var adapter = GetAdapter(adapterKey);
            using (var newDNS = adapter.GetMethodParameters("SetDNSServerSearchOrder"))
            {
                newDNS["DNSServerSearchOrder"] = dnsServers;
                adapter.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
            }
        }

        ~NetworkManager()
        {
            Manager.Dispose();
        }
    }
}
