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

        public void CreateFolder(string folderName)
        {
            try
            {
                factory.CreateFolder(folderName);
                Console.WriteLine("Create allowed.");
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Create. Error message : {0}", e.Message);
            }
        }

        public void CreateFile(string fileName, string text)
        {
            try
            {
                string encMessage = Encryp(text);

                factory.CreateFile(fileName, encMessage);
                Console.WriteLine("Create allowed.");
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Create. Error message : {0}", e.Message);
            }
        }
        public void Delete(string fileName)
        {
            try
            {
                factory.Delete(fileName);
                Console.WriteLine("Delete allowed.");
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Delete. Error message : {0}", e.Message);
            }
        }

        public void Rename(string oldFileName, string newFileName)
        {
            try
            {
                factory.Rename(oldFileName, newFileName);
                Console.WriteLine("Rename allowed.");
            }
            catch (SecurityAccessDeniedException e)
            {
                Console.WriteLine("Error while trying to Create. Error message : {0}", e.Message);
            }
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

        public static string Encryp(string decrypted)
        {
            string IV = "qo1lc3sjd8zpt9cx";  //16 chars = 128 bytes
            string Key = "ow7dxys8glfor9tnc2ansdfo1etkfjcv"; // 32 chars = 256 bytes
            byte[] textbytes = UnicodeEncoding.ASCII.GetBytes(decrypted);
            AesCryptoServiceProvider encdec = new AesCryptoServiceProvider();
            encdec.BlockSize = 128;
            encdec.KeySize = 256;
            encdec.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            encdec.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            encdec.Padding = PaddingMode.PKCS7;
            encdec.Mode = CipherMode.CBC;
            ICryptoTransform icrypt = encdec.CreateEncryptor(encdec.Key, encdec.IV);
            byte[] enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
            icrypt.Dispose();
            return Convert.ToBase64String(enc);
        }

    }
}
