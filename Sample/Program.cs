using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gmail_oauth2;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            oauth2.sync("yourmail@gmail.com", "client path (json)");

            Console.WriteLine(String.Format("Email: {0}\nAccessToken: {1}\nRefreshToken: {2}\nClient_Id: {3}\nClient_Secret: {4}", oauth2_account.mail_address, oauth2_account.oauth2_access_Token, oauth2_account.oauth2_refresh_Token, oauth2_account.client_id, oauth2_account.client_secret));
            oauth2.send_Gmail(new string[] { "to1@gmail.com", "to2@yahoo.co.jp" }, "test", "Heloooooooo", "gmail_oauth2", "path/image.png", "iamge", "png");
            Console.ReadKey();
        }
    }
}
