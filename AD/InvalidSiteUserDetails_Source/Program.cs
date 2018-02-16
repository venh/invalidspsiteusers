using System;
using System.Xml;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using InvalidSiteUserDetails.UserGroupProxy;
using System.DirectoryServices.AccountManagement;

namespace InvalidSiteUserDetails
{
    class Program
    {
        static string SITE_URL = string.Empty;
        static string USER_NAME = string.Empty;
        static string PASSWORD = string.Empty;
        static string DOMAIN = string.Empty;        

        static void Main(string[] args)
        {
            string accName = string.Empty;
            string userName = string.Empty;
            string usrIdName = string.Empty;
            UserGroup ugProxy = null;
            XmlNode usrsNode = null;
            XElement usrDoc = null;
            IEnumerable<XElement> userEles = null;
            string DASHED_LINE = "......................................................";
            string userDomain = string.Empty;
            ConsoleKeyInfo key;
            try
            {
                SITE_URL = ConfigurationManager.AppSettings["SITE_URL"];
                Console.Write("Please enter a valid user name: ");
                USER_NAME = Console.ReadLine();
                Console.Write("Please enter the password: ");
                do
                {
                    key = Console.ReadKey(true);
                    // Backspace Should Not Work
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        PASSWORD = string.Concat(PASSWORD, key.KeyChar);
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && PASSWORD.Length > 0)
                        {
                            PASSWORD = PASSWORD.Substring(0, (PASSWORD.Length - 1));
                            Console.Write("\b \b");
                        }
                    }
                }
                // Stops Receving Keys Once Enter is Pressed
                while (key.Key != ConsoleKey.Enter);
                Console.WriteLine();
                Console.Write("Please enter a valid domain: ");
                DOMAIN = Console.ReadLine();
                Console.WriteLine("Starting to get Invalid Users from the site.");
                ugProxy = new UserGroup();
                ugProxy.Credentials = new NetworkCredential(USER_NAME, PASSWORD, DOMAIN);
                ugProxy.Url = SITE_URL + "_vti_bin/usergroup.asmx";
                usrsNode = ugProxy.GetAllUserCollectionFromWeb();
                usrDoc = XElement.Parse(usrsNode.InnerXml);
                userEles = usrDoc.Descendants();                
                Console.WriteLine("Total User Count: " + userEles.Count());
                Console.WriteLine(DASHED_LINE);
                int failedCnt = 0;
                foreach (var userEle in userEles)
                {
                    usrIdName = userEle.Attribute(XName.Get("ID")).Value + ";#" + userEle.Attribute(XName.Get("Name")).Value;
                    accName = userEle.Attribute(XName.Get("LoginName")).Value;                   
                    userName = accName.Substring(accName.LastIndexOf('\\') + 1);
                    userDomain = accName.Replace("\\" + userName, string.Empty);
                    if (userDomain.ToUpperInvariant() == DOMAIN.ToUpperInvariant())
                    {
                        bool bln = CheckUserExists(userName);
                        if (!bln)
                        {
                            LogHelper.LogUserDetails(userName, userEle.Attribute("Name").Value, accName, userEle.Attribute("Email").Value);
                            Console.WriteLine("UserName: " + userName + " : IsValid: " + bln.ToString());
                            failedCnt++;
                        }
                    }
                }
                Console.WriteLine(DASHED_LINE);
                Console.WriteLine("Invalid user count: " + failedCnt);
                Console.WriteLine("Completed getting Invalid Users from the site.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
            finally
            {
                if (ugProxy != null)
                {
                    ugProxy.Dispose();
                    ugProxy = null;
                }
                usrsNode = null;
                usrDoc = null;
                userEles = null;
            }
        }

        private static bool CheckUserExists(string strUser)
        {
            bool? userExists;
            bool usrExists = false;
            try
            {
                using (var domainContext = new PrincipalContext(ContextType.Domain, DOMAIN))
                {
                    using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, strUser))
                    {
                        if (foundUser == null)
                        {
                            usrExists = false;
                            return usrExists;
                        }
                        userExists = ((AuthenticablePrincipal)(foundUser)).Enabled;
                        if (userExists != null)
                        {
                            if (userExists == false)
                            {
                                usrExists = false;
                            }
                            else
                            {
                                usrExists = true;
                            }
                        }
                        else
                        {
                            usrExists = false;
                        }
                    }
                }
                return usrExists;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        private static XmlNode GetXmlNode(XElement ele)
        {
            XmlDocument xD = new XmlDocument();
            xD.LoadXml(ele.ToString());
            XmlNode xN = xD.FirstChild;
            return xN;
        }
    }
}
