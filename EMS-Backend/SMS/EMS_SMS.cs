using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS_Backend.SMS
{
    public class EMS_SMS
    {
        public static object SendOTP(string Number, string ApiCode, string ApiPassword, string link)
        {
            object functionReturnValue = null;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                System.Collections.Specialized.NameValueCollection parameter = new System.Collections.Specialized.NameValueCollection();
                string url = "https://www.itexmo.com/php_api/api.php";
                parameter.Add("1", Number);
                parameter.Add("2", $"{FormatActiovationLinkMessage(link)}");
                parameter.Add("3", ApiCode);
                parameter.Add("passwd", ApiPassword);
                dynamic rpb = client.UploadValues(url, "POST", parameter);
                functionReturnValue = (new System.Text.UTF8Encoding()).GetString(rpb);
            }
            return functionReturnValue;
        }

        private static string FormatActiovationLinkMessage(string link)
        {
            //return $"Activate your account here: {link}";
            return $"Activate account here: {link}";
        }
    }
}
