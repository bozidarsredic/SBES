using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.ServiceModel.Security;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace ClientApp
{
	public class WCFClient : ChannelFactory<IWCFService>, IWCFService, IDisposable
	{
		IWCFService factory;

		public WCFClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
            Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;

            factory = this.CreateChannel();
		}


        public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}



        public string ShowFolderContent(string folderName)
        {
            string a = "";
            try
            {

                a = factory.ShowFolderContent(folderName);

            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Show. Error message : {0}", e.Message);
            }

            return a;
        }




    }
}
