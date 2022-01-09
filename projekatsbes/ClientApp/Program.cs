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

            while (true)
            {
                Console.WriteLine("Enter the number to select the method:");
                Console.WriteLine("1. Show Folder Content");
                Console.WriteLine("2. Read File");
                Console.WriteLine("3. Create Folder");
                Console.WriteLine("4. Create File");
                Console.WriteLine("5. Delete");
                Console.WriteLine("6. Rename");
                Console.WriteLine("7. Move To");
                Console.WriteLine("Enter q to exit");
                Console.WriteLine("Number:");
                string rez = Console.ReadLine();
                if (rez == "q") break;
                Console.WriteLine("-------------");



                using (WCFClient proxy = new WCFClient(binding, endpointAddress))
                {

                }
            }
                    Console.ReadLine();
		}
	}
}
