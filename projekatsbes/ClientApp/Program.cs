using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Security.Principal;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
           // Debugger.Launch();

			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/WCFService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;//korak 1
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Currently logged in:" + WindowsIdentity.GetCurrent().Name);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),
                EndpointIdentity.CreateUpnIdentity("wcfServer"));

         



			Console.ReadLine();
		}
	}
}
