using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailReaderMVC.Utils
{
    internal class Parser
    {

        public Parser() { }

        public static string Base64UrlToBase64(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            int mod4 = base64.Length % 4;
            if (mod4 > 0)
            {
                base64 += new string('=', 4 - mod4);
            }

            return base64;
        }

        public static string DecodeBase64(string base64Encoded)
        {
            byte[] data = Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(data);
        }

        public void logObjectProperties(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);
                Console.WriteLine($"{property.Name}: {value}\n");
            }
        }

    }
}
