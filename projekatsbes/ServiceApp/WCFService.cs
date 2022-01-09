using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.Security.Permissions;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.ServiceModel.Security;
using System.ServiceModel;
using SecurityManager;
using System.IO;
using System.Security.Cryptography;

namespace ServiceApp
{
	public class WCFService : IWCFService
	{


        public string ShowFolderContent(string folderName)
        {

            string b = "";


            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("see"))
            {
                Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5)}");


                var dir = Path.Combine(folderName);
                b = PrintDirectoryTree(dir, 2, new string[] { "folder3" });

                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "ShowFolderContent method need see permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call ShowFolderContent method. ShowFolderContent method need  see permission.");
            }


            return b;

        }

        public static string PrintDirectoryTree(string directory, int lvl, string[] excludedFolders = null, string lvlSeperator = "")
        {
            string a = "";
            excludedFolders = excludedFolders ?? new string[0];

            foreach (string f in Directory.GetFiles(directory))
            {
                //              Console.WriteLine(lvlSeperator + Path.GetFileName(f));
                a += lvlSeperator + Path.GetFileName(f) + "\n";
            }

            foreach (string d in Directory.GetDirectories(directory))
            {
                // Console.WriteLine(lvlSeperator + "-" + Path.GetFileName(d));
                a += lvlSeperator + Path.GetFileName(d) + "\n";

                if (lvl > 0 && Array.IndexOf(excludedFolders, Path.GetFileName(d)) < 0)
                {
                    PrintDirectoryTree(d, lvl - 1, excludedFolders, lvlSeperator + "  ");
                }
            }

            return a;
        }

        public string ReadFile(string fileName)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);
            string encMessage;

            if (Thread.CurrentPrincipal.IsInRole("see"))
            {
                Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5)}");


                if (File.Exists(fileName))
                {


                    string text = System.IO.File.ReadAllText(fileName);
                    //Console.WriteLine(text);

                    encMessage = Encryp(text);

                }
                else
                {

                    throw new FaultException("File doesn`t exist");
                }


                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "CreateFolder method need Change permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call CreateFolder method. CreateFolder method need  Change permission.");
            }

            return encMessage;
        }


        public void CreateFolder(string folderName)
        {
            Console.WriteLine("Currently logged in:" + WindowsIdentity.GetCurrent().Name);

            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("change"))
            {

                //  Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name}");
                Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5)}");


                var absolute_path = Path.Combine(folderName);

                if (!System.IO.Directory.Exists(folderName))
                {
                    System.IO.Directory.CreateDirectory(folderName);
                }


                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "CreateFolder method need Change permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call CreateFolder method. CreateFolder method need  Change permission.");
            }



        }

        public void CreateFile(string fileName, string text)
        {

            string a = "";
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("change"))
            {
                //  string name = WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5);
                Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5)}");
                try
                {
                    string key = "ow7dxys8glfor9tnc2ansdfo1etkfjcv";

                    string ivstring = "qo1lc3sjd8zpt9cx";
                    ;
                    a = AES_Decrypt_CBC(text, key, ivstring);
                    Console.WriteLine("kriptovan:" + text);
                    Console.WriteLine("dekriptovan:" + a);
                    StreamWriter sw = File.CreateText(fileName);
                    sw.WriteLine(a);

                    sw.Close();



                }
                catch (Exception e)
                {
                    throw new FaultException<SecurityException>(new SecurityException(e.Message));
                }


                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "CreateFile method need Change permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call CreateFile method. CreateFile method need  Change permission.");
            }
        }

        public void Delete(string fileName)
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("delete"))
            {

                Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5)}");

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    Console.WriteLine("File deleted");

                }
                else
                {

                    throw new FaultException("File doesn`t exist");
                }
                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Delete method need Delete permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call Delete method. Delete method need  Delete permission.");
            }
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public void Rename(string oldFileName, string newFileName)
        {


            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
            string userName = Formatter.ParseName(principal.Identity.Name);

            if (Thread.CurrentPrincipal.IsInRole("change"))
            {
                //  Console.WriteLine($"Process Identity:{WindowsIdentity.GetCurrent().Name}");
                string a = WindowsIdentity.GetCurrent().Name.Substring(5, WindowsIdentity.GetCurrent().Name.Length - 5);
                Console.WriteLine($"Process Identity:{a}");


                try
                {


                    System.IO.File.Move(oldFileName, newFileName);

                }
                catch (Exception)
                {
                    throw new FaultException("File doesn`t exist");


                }


                try
                {
                    Audit.AuthorizationSuccess(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                try
                {
                    Audit.AuthorizationFailed(userName,
                        OperationContext.Current.IncomingMessageHeaders.Action, "Rename method need Change permission.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                throw new FaultException("User " + userName +
                    " try to call Rename method. Rename method need  Change permission.");
            }

        }
        //OVDJE DODAJ


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
