using System;
using System.Threading;
using Google.Apis;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;

namespace gmail_oauth2
{
    class Program
    {
        static void Main(string[] args)
        {
            MailMessage msg = new MailMessage();
            //System.Net.Mail.MailMessage

            msg.From = new MailAddress("from@gmail.com");
            msg.To.Add("to@gmail.com");
            /*
             * send Mail to multiple people
             *  msg.To.Add("to1@gmail.com,to2@docomo.ne.jp,to3@yahoo.co.jp");
             */

            //Subject
            msg.Subject = "test subject";
            //text
            msg.Body = "test text";

            //Attachments
            Attachment attachment = new Attachment("your attachment path");

            //attachment.ContentType = new ContentType("image/png");
            attachment.ContentType = new ContentType("content type");

            msg.Attachments.Add(attachment);


            //SMTP authentication
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("from@gmail.com", "password or oauth2.0 App password");
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            client.Send(msg);
        }

        static string OAuth2()
        {
            /*
             *Implementation soon 
             */
            return "";
        }
    }
}
