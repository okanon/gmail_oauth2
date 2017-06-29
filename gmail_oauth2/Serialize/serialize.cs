using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace gmail_oauth2.Serialize
{
    public class oauth_serialize
    {
        public string client_id;
        public string client_secret;
        public string oauth2_access_token;
        public string oauth2_refresh_token;

        public oauth_serialize()
        {
            this.client_id = null;
            this.client_secret = null;
            this.oauth2_access_token = null;
            this.oauth2_refresh_token = null;
        }

        public oauth_serialize(string id, string secret, string atoken, string rtoken)
        {
            this.client_id = id;
            this.client_secret = secret;
            this.oauth2_access_token = atoken;
            this.oauth2_refresh_token = rtoken;
        }
    }

    public class xml_serialixation
    {
        public static void oauth2_certification(Type[] et, oauth_serialize ary, string path = @"/oauth2_certificate.xml")
        {
            XmlSerializer arg_2 = new XmlSerializer(typeof(oauth_serialize), et);
            StreamWriter streamWriter = new StreamWriter(Directory.GetCurrentDirectory() + path, false, new UTF8Encoding(false));
            arg_2.Serialize(streamWriter, ary);
            streamWriter.Close();
        }
    }

    public class load_xml
    {
        public static oauth_serialize load_oauth2(string path = @"/oauth2_certificate.xml")
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(oauth_serialize), new Type[]
            {
                typeof(oauth_serialize)
            });
            oauth_serialize result;
            using (StreamReader streamReader = new StreamReader(Directory.GetCurrentDirectory() + path, new UTF8Encoding(false)))
            {
                result = (oauth_serialize)xmlSerializer.Deserialize(streamReader);
            }
            return result;
        }
    }
}
