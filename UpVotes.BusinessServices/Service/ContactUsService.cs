using System;
using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;
using System.Linq;
using System.Threading;

namespace UpVotes.BusinessServices.Service
{
    public class ContactUsService : IContactUsService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }        

        private UpVotesEntities _context = null;        

        public int SaveContactUsInformation(ContactUsInfoEntity ContactRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                   var contact = _context.Sp_InsContactUsInfo(ContactRequest.Name, ContactRequest.Email, ContactRequest.Phone, ContactRequest.CompanyName, ContactRequest.ContactMessage, ContactRequest.AddedBy).FirstOrDefault();
                    if(contact > 0)
                    {
                        Thread threadAck = new Thread(() => SendContactInfoToAdmin(ContactRequest.Name, ContactRequest.Email, ContactRequest.Phone, ContactRequest.CompanyName, ContactRequest.ContactMessage));
                        threadAck.Start();
                        return Convert.ToInt32(contact);
                    }
                    else {
                        return 0;
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void SendContactInfoToAdmin(string Name, string Email, string Phone, string CompanyName, string ContactMessage)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = "You have received a new message from the enquiries form on your website.";
            emailProperties.EmailBody = GetContactInfoEmailContent(Name, Email, Phone, CompanyName, ContactMessage).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetContactInfoEmailContent(string Name, string Email, string Phone, string CompanyName, string ContactMessage)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello Upvotes,</p><p> We have received a new message from the enquiries form on your website.</p>");
            sb.Append("<p>Details of the user:</p>");
            sb.Append("<p>Name: " + Name + "</p>");
            sb.Append("<p>Company: " + CompanyName + "</p>");
            sb.Append("<p>Email: " + Email + "</p>");
            sb.Append("<p>Phone: " + Phone + "</p>");
            sb.Append("<p>Message: " + ContactMessage + "</p>");            
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }
    }
}
