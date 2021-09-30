using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMS_Backend.SMTP
{
    public class EMS_SMTP
    {
        public static bool SendActivationCode(string applicantName, string url, string sender, string alias, string password, string emailAddress)
        {
            try
            {
                SmtpClient mailClient = new SmtpClient("smtp.gmail.com", 587);
                MailMessage msgMail = new MailMessage();
                msgMail.From = new MailAddress(sender, alias);
                mailClient.UseDefaultCredentials = false;
                mailClient.Credentials = new NetworkCredential(sender, password);
                mailClient.EnableSsl = true;
                MailAddress sendMailTo = new MailAddress(emailAddress);

                msgMail.To.Add(sendMailTo);
                msgMail.Subject = "Account Activation";
                msgMail.IsBodyHtml = true;
                msgMail.Body = BodyTemplate(applicantName, url);

                mailClient.Send(msgMail);
                msgMail.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        private static string BodyTemplate(string applicantName, string url)
        {
            return  $" <b>{DateTime.Now.ToString("MMMM dd, yyyy")} </b>" +
                    "<br><br>" +
                    $"Dear {applicantName.ToUpper()}," +
                    $"<br><b>You application as member has been approved. You can activate your account by clicking <a href='{ url }'>Here</a></b>" +
                    "<br This activation link is only valid for the next 24hours."+
                    "<br> Should you have any inquiries regarding your account, please contact the" + 
                    "<p style = 'color:blue;'><b>EMS Support</b></p>" +
                    "<i>** This Electronic Mail is system-generated. Please do not reply. **</i>";
        }


    }
}
