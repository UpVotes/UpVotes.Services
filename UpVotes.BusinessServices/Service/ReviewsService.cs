﻿using System;
using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;
using System.Linq;
using System.Threading;

namespace UpVotes.BusinessServices.Service
{
    public class ReviewsService : IReviewsService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }        

        private UpVotesEntities _context = null;        

        public bool AddCompanyReview(CompanyReviewsEntity companyReviewEntity)
        {
            using (_context = new UpVotesEntities())
            {
                try
                {                    
                    var isReview = _context.Sp_InsCompanyReview(companyReviewEntity.CompanyID, companyReviewEntity.FocusAreaID, companyReviewEntity.UserID, companyReviewEntity.ReviewerCompanyName, companyReviewEntity.ReviewerDesignation, companyReviewEntity.ReviewerProjectName, companyReviewEntity.FeedBack, companyReviewEntity.Rating, companyReviewEntity.Email, companyReviewEntity.Phone, companyReviewEntity.ReviewerFullName).FirstOrDefault();
                    if (isReview > 0)
                    {
                        Thread threadAck = new Thread(() => SendAcknowledgeMail(companyReviewEntity.CompanyName, companyReviewEntity.ReviewerDesignation, companyReviewEntity.ReviewerCompanyName, companyReviewEntity.ReviewerFullName));
                        Thread threadThx = new Thread(() => SendThankyouMail(companyReviewEntity.CompanyName, companyReviewEntity.ReviewerDesignation, companyReviewEntity.ReviewerCompanyName, companyReviewEntity.ReviewerFullName, companyReviewEntity.Email));
                        threadAck.Start();
                        threadThx.Start();
                        //SendAcknowledgeMail(companyReviewEntity.CompanyName, companyReviewEntity.ReviewerDesignation, companyReviewEntity.ReviewerCompanyName, companyReviewEntity.ReviewerFullName);
                        //SendThankyouMail(companyReviewEntity.CompanyName, companyReviewEntity.ReviewerDesignation, companyReviewEntity.ReviewerCompanyName, companyReviewEntity.ReviewerFullName, companyReviewEntity.Email);
                        return true;
                    }
                    else
                    {
                        return false;
                    }                    
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public AutocompleteResponseEntity GetSoftwareCompanyAutoCompleteData(AutocompleteRequestEntity SoftwareCompanyAutoCompleteRequest)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    AutocompleteResponseEntity myAutoCompleteList = new AutocompleteResponseEntity();
                    myAutoCompleteList.companySoftwareAutocomplete = new List<Autocomplete>();
                    var result = _context.Sp_GetCompanySoftwareNamesForAutoComplete(SoftwareCompanyAutoCompleteRequest.Type, SoftwareCompanyAutoCompleteRequest.Search).ToList();
                    if (result != null)
                    {
                        result.ToList().ForEach(q => myAutoCompleteList.companySoftwareAutocomplete.Add(new Autocomplete
                        {
                            ID = q.ID,
                            Name = q.Name
                        }));
                    }
                    return myAutoCompleteList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AddSoftwareReview(SoftwareReviewsEntity softwareReviewEntity)
        {
            using (_context = new UpVotesEntities())
            {
                try
                {
                    var isReview = _context.Sp_InsSoftwareReview(softwareReviewEntity.SoftwareID, softwareReviewEntity.ServiceCategoryID, softwareReviewEntity.UserID, softwareReviewEntity.ReviewerCompanyName, softwareReviewEntity.ReviewerDesignation, softwareReviewEntity.FeedBack, softwareReviewEntity.Rating, softwareReviewEntity.Email, softwareReviewEntity.Phone, softwareReviewEntity.ReviewerFullName).FirstOrDefault();
                    if (isReview > 0)
                    {
                        Thread threadAck = new Thread(() => SendAcknowledgeMail(softwareReviewEntity.SoftwareName, softwareReviewEntity.ReviewerDesignation, softwareReviewEntity.ReviewerCompanyName, softwareReviewEntity.ReviewerFullName));
                        Thread threadThx = new Thread(() => SendThankyouMail(softwareReviewEntity.SoftwareName, softwareReviewEntity.ReviewerDesignation, softwareReviewEntity.ReviewerCompanyName, softwareReviewEntity.ReviewerFullName, softwareReviewEntity.Email));
                        threadAck.Start();
                        threadThx.Start();
                        //SendAcknowledgeMail(softwareReviewEntity.SoftwareName, softwareReviewEntity.ReviewerDesignation, softwareReviewEntity.ReviewerCompanyName, softwareReviewEntity.ReviewerFullName);
                        //SendThankyouMail(softwareReviewEntity.SoftwareName, softwareReviewEntity.ReviewerDesignation, softwareReviewEntity.ReviewerCompanyName, softwareReviewEntity.ReviewerFullName, softwareReviewEntity.Email);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private void SendAcknowledgeMail(string Name, string ReviewerDesignation, string ReviewerCompanyName, string ReviewerFullName)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = "New review for " + Name + " from " + ReviewerDesignation + " at "+ ReviewerCompanyName;
            emailProperties.EmailBody = GetAcknowledgeEmailContent(Name, ReviewerDesignation, ReviewerCompanyName, ReviewerFullName).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private void SendThankyouMail(string Name, string ReviewerDesignation, string ReviewerCompanyName, string ReviewerFullName,string Email)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = Email;
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = "Thank you for review to "+ Name;
            emailProperties.EmailBody = GetThankyouEmailContent(Name, ReviewerDesignation, ReviewerCompanyName, ReviewerFullName).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAcknowledgeEmailContent(string Name, string ReviewerDesignation, string ReviewerCompanyName, string ReviewerFullName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello "+ Name+ ",</p><p> We have got you new review for your company profile from one of your customer side.</p>");
            sb.Append("<p>Details of the reviewer:</p>");
            sb.Append("<p>Name: "+ ReviewerFullName + "</p>");
            sb.Append("<p>Company: " + ReviewerCompanyName + "</p>");
            sb.Append("<p>Designation: " + ReviewerDesignation + "</p>");
            sb.Append("<p>Please use this <a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "' target = '_blank'>" + "Upvotes.co" + "</a> to approve or disapprove the reviews.");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetThankyouEmailContent(string Name, string ReviewerDesignation, string ReviewerCompanyName, string ReviewerFullName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello " + ReviewerFullName + ",</p><p> Thank you for valuable review to "+ Name + "</p>");
            sb.Append("<p>We will let you know once it is approved from review process.</p>");            
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

    }
}