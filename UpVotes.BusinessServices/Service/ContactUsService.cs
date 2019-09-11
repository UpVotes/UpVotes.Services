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

        public int SaveSponsorerInformation(SponsorerInfoEntity SponsorerRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    var sponsorerid = _context.Sp_InsSponsorshipRequest(SponsorerRequest.Name, SponsorerRequest.CompanyName, SponsorerRequest.Email, SponsorerRequest.Sponsorship, SponsorerRequest.UserDescription, SponsorerRequest.AddedBy);
                    if (sponsorerid > 0)
                    {
                        Thread threadAck = new Thread(() => SendSponsorerInfoToAdmin(SponsorerRequest.Name, SponsorerRequest.CompanyName, SponsorerRequest.Email, SponsorerRequest.Sponsorship, SponsorerRequest.UserDescription));
                        threadAck.Start();
                        Thread threadAck1 = new Thread(() => SendAckToSponsorer(SponsorerRequest.Email, SponsorerRequest.Name));
                        threadAck1.Start();
                        return sponsorerid;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void SendSponsorerInfoToAdmin(string Name, string CompanyName, string Email, string Sponsorship, string UserDescription)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = Email;
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = "New "+ Sponsorship + " User Subscription request ";
            emailProperties.EmailBody = GetSponsorerInfoEmailContent(Name, CompanyName, Email, Sponsorship, UserDescription).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private void SendAckToSponsorer(string Email, string Name)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = Email;
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = "Thank you";
            emailProperties.EmailBody = GetSponsorerAckEmailContent(Email, Name).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetSponsorerAckEmailContent(string Email, string Name)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello "+ Name + ",</p><p>Thanks for reaching out!</p>");
            sb.Append("<p>Our team will review and get back to you within one business day.</p>");            
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
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

        private string GetSponsorerInfoEmailContent(string Name, string CompanyName, string Email, string Sponsorship, string UserDescription)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello Upvotes,</p><p>The below user has requested for new subscription.</p>");
            sb.Append("<p>Details of the user:</p>");
            sb.Append("<p>Name: " + Name + "</p>");
            sb.Append("<p>Company: " + CompanyName + "</p>");
            sb.Append("<p>Email: " + Email + "</p>");
            sb.Append("<p>SponsorShip: " + Sponsorship + "</p>");
            sb.Append("<p>Description: " + UserDescription + "</p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
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
