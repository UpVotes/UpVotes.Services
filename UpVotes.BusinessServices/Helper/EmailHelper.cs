using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UpVotes.DataModel;

namespace UpVotes.BusinessServices
{
    public class EmailHelper
    {
        public static bool SendEmail(Email emailProp)
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    MailAddress mailAddress = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);

                    using (MailMessage message = new MailMessage())
                    {
                        message.IsBodyHtml = true;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        message.From = new MailAddress(emailProp.EmailFrom.Trim(), emailProp.DomainDisplayName);
                        GetEmailTo(emailProp, message);
                        GetEmailCc(emailProp, message);
                        GetEmailBcc(emailProp, message);
                        message.Subject = emailProp.EmailSubject;
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Priority = MailPriority.Normal;
                        message.Body = emailProp.EmailBody;

                        smtpClient.Host = "smtpout.secureserver.net";
                        smtpClient.Port = 80;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = false;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"], System.Configuration.ConfigurationManager.AppSettings["AdminPassword"]);
                        smtpClient.Credentials = credentials;
                        smtpClient.Send(message);
                        SaveEmail(emailProp);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void GetEmailTo(Email emailProp, MailMessage message)
        {
            if (emailProp.EmailTo != string.Empty && emailProp.EmailTo != null)
            {
                foreach (string emailTo in emailProp.EmailTo.Split(';'))
                {
                    message.To.Add(new MailAddress(emailTo.Trim()));
                }
            }
        }

        private static void GetEmailBcc(Email emailProp, MailMessage message)
        {
            if (emailProp.EmailBCC != string.Empty && emailProp.EmailBCC != null)
            {
                foreach (string emailBcc in emailProp.EmailBCC.Split(';'))
                {
                    message.Bcc.Add(new MailAddress(emailBcc.Trim()));
                }
            }
        }

        private static void GetEmailCc(Email emailProp, MailMessage message)
        {
            if (emailProp.EmailCC != string.Empty && emailProp.EmailCC != null)
            {
                foreach (string emailCC in emailProp.EmailCC.Split(';'))
                {
                    message.CC.Add(new MailAddress(emailCC.Trim()));
                }
            }
        }

        private static void SaveEmail(Email emailProp)
        {
            using (UpVotesEntities _upvoteContext = new UpVotesEntities())
            {
                _upvoteContext.Email.Add(emailProp);
                _upvoteContext.SaveChanges();
            }
        }

        public static void GetEmailSignature(System.Text.StringBuilder sb)
        {
            sb.Append("</br><p><strong><u> Thanks & Regards </u></strong></p>");
            sb.Append("<p><strong> Admin </strong></p>");
            sb.Append("<p><a href = 'mailto:" + System.Configuration.ConfigurationManager.AppSettings["AdminEmail"] + "' > support@upvotes.co </a></p>");
            sb.Append("<p><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "' > www.upvotes.co </a></p>");
            sb.Append("<p>Follow us on - <a href = 'https://www.linkedin.com/company/upvotes/'> LinkedIn </a> &nbsp;&nbsp;|&nbsp;&nbsp; <a href = 'https://twitter.com/upvotes_co'> Twitter </a> &nbsp;&nbsp;|&nbsp;&nbsp;<a href = 'https://www.facebook.com/upvotes.co/'> Facebook </a></p>");
        }
    }
}
