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

        public string ReadFile(string fileName)
        {
            string rez3 = "";
            try
            {
                string encMessage = factory.ReadFile(fileName);
                Console.WriteLine("Crypted Content:");
                Console.WriteLine(encMessage);


                string key = "ow7dxys8glfor9tnc2ansdfo1etkfjcv";

                string ivstring = "qo1lc3sjd8zpt9cx";

                rez3 = AES_Decrypt_CBC(encMessage, key, ivstring);


            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to ReadFile. Error message : {0}", e.Message);
            }

            return rez3;
        }


        //IZNAD METODE

        public static string AES_Decrypt_CBC(string cipherData, string keyString, string ivString)
        {
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            byte[] iv = Encoding.UTF8.GetBytes(ivString);

            try
            {
                using (var rijndaelManaged =
                new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
                using (var memoryStream =
                new MemoryStream(Convert.FromBase64String(cipherData)))
                using (var cryptoStream =
                new CryptoStream(memoryStream,
                rijndaelManaged.CreateDecryptor(key, iv),
                CryptoStreamMode.Read))
                {
                    return new StreamReader(cryptoStream).ReadToEnd();
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
            // You may want to catch more exceptions here...
        }

    }
}
