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

                    if (rez == "1")
                    {
                        Console.WriteLine("Enter Folder name:");
                        string rez2 = Console.ReadLine();
                        string a = proxy.ShowFolderContent(rez2);
                        Console.WriteLine("List:\n");
                        Console.WriteLine(a);
                    }
                    else if (rez == "2")
                    {
                        Console.WriteLine("Enter File name:");
                        string rez2 = Console.ReadLine();
                        string b = proxy.ReadFile(rez2);
                        Console.WriteLine("Content:\n");
                        Console.WriteLine(b);
                    }

                    else if (rez == "3")
                    {
                        Console.WriteLine("Enter Folder name:");
                        string rez2 = Console.ReadLine();
                        proxy.CreateFolder(rez2);
                    }

                    else if (rez == "4")
                    {
                        Console.WriteLine("Enter file name:");
                        string rez2 = Console.ReadLine();
                        Console.WriteLine("Enter file text:");
                        string rez3 = Console.ReadLine();
                        proxy.CreateFile(rez2, rez3);
                    }

                }
            }
                    Console.ReadLine();
		}
	}
}
