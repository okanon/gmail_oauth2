using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codeplex.Data;
using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MimeKit;

using gmail_oauth2.Serialize;

namespace gmail_oauth2
{
    public class oauth2
    {
        public static void sync(string gmail, string oauth2_client_path)
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"/TumblingDiceAnalytics/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"))
            {
                StreamReader stream = new StreamReader(oauth2_client_path, System.Text.Encoding.GetEncoding("utf-8"));
                string result = stream.ReadToEnd();
                dynamic json = DynamicJson.Parse(result);
                stream.Close();

                string[] scopes = new string[] {
                    "https://mail.google.com/",
                    "https://www.googleapis.com/auth/gmail.compose",
                    "https://www.googleapis.com/auth/gmail.insert",
                    "https://www.googleapis.com/auth/gmail.labels",
                    "https://www.googleapis.com/auth/gmail.metadata",
                    "https://www.googleapis.com/auth/gmail.modify",
                    "https://www.googleapis.com/auth/gmail.readonly",
                    "https://www.googleapis.com/auth/gmail.send",
                    "https://www.googleapis.com/auth/gmail.settings.basic",
                    "https://www.googleapis.com/auth/gmail.settings.sharing",
                };

                UserCredential c = acquisition_oauth2(oauth2_client_path, scopes);

                oauth_serialize oauth = new oauth_serialize(json.installed.client_id, json.installed.client_secret, c.Token.AccessToken, c.Token.RefreshToken);
                xml_serialixation.oauth2_certification(new Type[]
                {
                typeof(oauth_serialize)
                }, oauth);

                oauth2_account.oauth2_access_Token = c.Token.AccessToken;
                oauth2_account.oauth2_refresh_Token = c.Token.RefreshToken;
                oauth2_account.client_id = json.installed.client_id;
                oauth2_account.client_secret = json.installed.client_secret;
                oauth2_account.mail_address = gmail;
            }
            else
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"/oauth2_certificate.xml"))
                {
                    oauth_serialize oauth = load_xml.load_oauth2();

                    oauth2_account.oauth2_access_Token = oauth.oauth2_access_token;
                    oauth2_account.oauth2_refresh_Token = oauth.oauth2_refresh_token;
                    oauth2_account.client_id = oauth.client_id;
                    oauth2_account.client_secret = oauth.client_secret;
                    oauth2_account.mail_address = gmail;

                    refresh_oauth2token();
                }
            }
        }

        public static void send_Gmail(string[] To, string Subject, string Body, string From_name = "", string attachment_path = null, string ContentMediaType = null, string ContentSubType = null)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(From_name, oauth2_account.mail_address));
            
            foreach (string str in To)
            {
                message.To.Add(new MailboxAddress("", str));
            }

            message.Subject = Subject;

            

            if (ContentMediaType != null)
            {
                var body = new TextPart("plain")
                {
                    Text = Body
                };

                var attachment = new MimePart(ContentMediaType, ContentSubType)
                {
                    ContentObject = new ContentObject(File.OpenRead(attachment_path), ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(attachment_path)
                };

                var multipart = new Multipart("mixed");
                multipart.Add(body);
                multipart.Add(attachment);

                message.Body = multipart;
            }
            else
            {
                message.Body = new TextPart("plain")
                {
                    Text = Body
                };
            }

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                //client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(oauth2_account.mail_address, oauth2_account.oauth2_access_Token);

                client.Send(message);
                client.Disconnect(true);

            }
        }

        public static void refresh_oauth2token()
        {
            string data = string.Format("client_secret={0}&grant_type=refresh_token&refresh_token={1}&client_id={2}", oauth2_account.client_secret, oauth2_account.oauth2_refresh_Token, oauth2_account.client_id);
            byte[] byte_data = Encoding.ASCII.GetBytes(data);

            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(oauth2_access.token_uri);
            requests.Method = "POST";
            requests.ContentType = "application/x-www-form-urlencoded";
            requests.ContentLength = byte_data.Length;
            Stream requestStream = requests.GetRequestStream();
            requestStream.Write(byte_data, 0, byte_data.Length);
            requestStream.Close();
            HttpWebResponse res = null;

            try
            {
                res = (HttpWebResponse)requests.GetResponse();
            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception);
            }

            Stream resStream = res.GetResponseStream();
            string rawJson = new StreamReader(res.GetResponseStream()).ReadToEnd();
            dynamic json = DynamicJson.Parse(rawJson);
            //{"access_token" : "XXXX", "expires_in" : 3600, "token_type" : "Bearer"}

            oauth_serialize oauth = new oauth_serialize(oauth2_account.client_id, oauth2_account.client_secret, json.access_token, oauth2_account.oauth2_refresh_Token);
            xml_serialixation.oauth2_certification(new Type[]
            {
                typeof(oauth_serialize)
            }, oauth);

            oauth2_account.oauth2_access_Token = json.access_token;
        }

        public static UserCredential acquisition_oauth2(string oauth2_client_path, string[] Scopes)
        {
            UserCredential credential;

            using (var stream =
                new FileStream(oauth2_client_path, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(Directory.GetCurrentDirectory() + @"/TumblingDiceAnalytics", false)).Result;
                // %CurrentDirectory%/TumblingDiceAnalytics
            }

            oauth2_account.oauth2_refresh_Token = credential.Token.RefreshToken;
            oauth2_account.oauth2_access_Token = credential.Token.AccessToken;

            return credential;
        }

        public static void oauth2_code(string scope_string)
        {
            string uri = String.Format("?client_id={0}&redirect_uri={1}&scope={2}&response_type=code&approval_prompt=force&access_type=offline", oauth2_account.client_id, oauth2_access.redirect_uri, scope_string);

            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(oauth2_access.auth_uri + uri);
            requests.Method = "POST";
            Stream requestStream = requests.GetRequestStream();
            requestStream.Close();
            HttpWebResponse res = null;

            try
            {
                res = (HttpWebResponse)requests.GetResponse();
            }
            catch (Exception Exception)
            {
                Console.WriteLine(Exception);
            }

            Process.Start(res.ResponseUri.ToString());
            Console.Write("認証を済ませたら、コードをコピーして入力してください。>>>");
            oauth2_account.code = Console.ReadLine();
        }
    }

    public class oauth2_access
    {
        internal static string auth_uri
        {
            get { return "https://accounts.google.com/o/oauth2/auth"; }
        }

        internal static string token_uri
        {
            get { return "https://accounts.google.com/o/oauth2/token"; }
        }

        internal static string auth_provider_x509_cert_url
        {
            get { return "https://www.googleapis.com/oauth2/v1/certs"; }
        }

        internal static string redirect_uri
        {
            get { return "urn:ietf:wg:oauth:2.0:oob"; }
        }

        internal static string oauthplayback_uri
        {
            get { return "https://developers.google.com/oauthplayground/"; }
        }
    }

    public class oauth2_account
    {
        public static string oauth2_access_Token
        {
            get; set;
        }

        public static string oauth2_refresh_Token
        {
            get; set;
        }

        public static string client_id
        {
            get; set;
        }

        public static string client_secret
        {
            get; set;
        }

        public static string code
        {
            get; set;
        }

        public static string mail_address
        {
            get; set;
        }
    }
}
