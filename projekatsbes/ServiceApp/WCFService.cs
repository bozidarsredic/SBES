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

    }
}
