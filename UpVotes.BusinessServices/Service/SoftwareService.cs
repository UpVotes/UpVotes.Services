﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessEntities.Helper;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class SoftwareService : ISoftwareService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }

        UpVotesEntities _context = null;
        

        public SoftwareDetail GetAllSoftwareDetails(int? serviceCategoryID, string softwareName, string sortby, int userID = 0, int PageNo = 1, int PageSize = 10)
        {
            SoftwareDetail softwareDetail = new SoftwareDetail();
            OverviewNewsService newsObj = new OverviewNewsService();
            softwareDetail.SoftwareList = new List<SoftwareEntity>();
            softwareName = Helper.BasicDecryptString(softwareName);

            try
            {
                using (_context = new UpVotesEntities())
                {
                    IEnumerable<SoftwareEntity> softwareEntities = GetSoftware(serviceCategoryID, softwareName, userID, PageNo, PageSize, sortby).ToList();
                    if (softwareEntities.Count() > 0)
                    {
                        foreach (SoftwareEntity software in softwareEntities)
                        {
                            if (softwareName != "")
                            {
                                software.SoftwareName = System.Web.HttpUtility.HtmlEncode(software.SoftwareName);
                                software.SoftwareReviews = GetSoftwareReviews(software.SoftwareName,5).ToList();
                                software.OverviewNewsData = newsObj.GetCompanySoftwareNewsByID(2, software.SoftwareID);
                                if (software.SoftwareReviews.Count() > 0)
                                {
                                    foreach (SoftwareReviewsEntity softwareReviewsEntity in software.SoftwareReviews)
                                    {
                                        softwareReviewsEntity.NoOfThankNotes = GetSoftwareReviewThankNotes(software.SoftwareID, softwareReviewsEntity.SoftwareReviewID).Count();
                                    }
                                }
                            }
                            softwareDetail.SoftwareList.Add(software);
                        }
                    }
                    //if (softwareEntities != null)
                    //{
                    //    softwareEntities.ToList().ForEach(q => softwareDetail.SoftwareList.Add(new SoftwareEntity
                    //    {
                    //        SoftwareID = Convert.ToInt32(q.SoftwareID),
                    //        CreatedBy = Convert.ToInt32(q.CreatedBy),
                    //        SoftwareName = q.SoftwareName,
                    //        LogoName = q.LogoName,
                    //        TagLine = q.TagLine,
                    //        UserRating = Convert.ToInt32(q.UserRating),
                    //        NoOfUsersRated = Convert.ToInt32(q.NoOfUsersRated),
                    //        NoOfVotes = Convert.ToInt32(q.NoOfVotes),
                    //        IsVoted = Convert.ToBoolean(q.IsVoted),
                    //        PriceRange = q.PriceRange,
                    //        SoftwareTrail = q.SoftwareTrail,
                    //        DemoURL = q.DemoURL,
                    //        DomainID = q.DomainID,
                    //        WebSiteURL = q.WebSiteURL,
                    //        LinkedInURL = q.LinkedInURL,
                    //        FaceBookURL = q.FaceBookURL,
                    //        TwitterURL = q.TwitterURL,
                    //        InstagramURL = q.InstagramURL,
                    //        FoundedYear = Convert.ToInt32(q.FoundedYear),
                    //        SoftwareDescription = q.SoftwareDescription,
                    //        CreatedDate = q.CreatedDate == null ? string.Empty : Convert.ToDateTime(q.CreatedDate).ToString("dd-MMM-yyyy"),
                    //        ServiceCategoryName = q.ServiceCategoryName,
                    //        ModifiedBy = Convert.ToInt32(q.ModifiedBy),
                    //        ModifiedDate = q.ModifiedDate == null ? string.Empty : Convert.ToDateTime(q.ModifiedDate).ToString("dd-MMM-yyyy"),
                    //        ServiceCategoryID = Convert.ToInt32(q.ServiceCategoryID),
                    //        IsActive = Convert.ToBoolean(q.IsActive),
                    //        Ranks = Convert.ToInt32(q.Ranks),
                    //        TotalCount = Convert.ToInt32(q.TotalCount),
                    //        IsClaim = Convert.ToBoolean(q.IsClaim)
                    //    }
                    //    ));
                    //}
                    return softwareDetail;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SoftwareDetail GetUserReviewsForSoftware(string softwareName, int noOfRows)
        {
            SoftwareDetail softwareDetail = new SoftwareDetail();
            softwareDetail.SoftwareList = new List<SoftwareEntity>();

            SoftwareEntity software = new SoftwareEntity();
            software.SoftwareName = softwareName.Trim();
            software.SoftwareReviews = GetSoftwareReviews(software.SoftwareName, noOfRows).ToList();

            softwareDetail.SoftwareList.Add(software);

            return softwareDetail;
        }

        public CompanySoftwareReviews GetReviewsForSoftwareListingPage(SoftwareFilterEntity softwareFilter)
        {
            CompanySoftwareReviews softwareReviewsObj = new CompanySoftwareReviews();
            softwareReviewsObj.SoftwareReviewsList = new List<SoftwareReviewsEntity>();
            softwareReviewsObj.SoftwareNamesList = new List<string>();


            using (_context = new UpVotesEntities())
            {
                var response = _context.Sp_GetSoftwareReviewsForListingPage(softwareFilter.SoftwareName, softwareFilter.SoftwareCategoryId, 5, softwareFilter.PageNo, softwareFilter.PageSize).ToList();
                if (response != null)
                {
                    response.ToList().ForEach(q => softwareReviewsObj.SoftwareReviewsList.Add(new SoftwareReviewsEntity
                    {
                        SoftwareReviewID = q.SoftwareReviewID,
                        SoftwareID = q.SoftwareId,
                        SoftwareName = q.SoftwareName,
                        ReviewerFullName = q.ReviewerFullName,
                        ReviewerSoftwareName = q.ReviewerSoftwareName,
                        ReviewerDesignation = q.ReviewerDesignation,
                        ReviewerProjectName = q.ReviewerProjectName,
                        ServiceCategoryName = q.ServiceCategoryName,
                        FeedBack = q.FeedBack,
                        Rating = q.Rating,
                        ReviewDate = q.ReviewDate,
                        UserProfilePicture = q.UserProfilePicture,
                        NoOfReviews = q.NoOfReviews,
                        NoOfThankNotes = q.ThankYouCount
                    }
                ));
                    softwareReviewsObj.SoftwareNamesList = response.ToList().Select(i => i.SoftwareName).Distinct().ToList();

                }
            }

            return softwareReviewsObj;
        }

        public string ThanksNoteForSoftwareReview(SoftwareReviewThankNoteEntity softwareReviewThanksNoteEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {

                    var isThankNote = _context.Sp_InsSoftwareReviewForThanksNote(softwareReviewThanksNoteEntity.UserID, softwareReviewThanksNoteEntity.SoftwareReviewID).FirstOrDefault();

                    if (isThankNote == -1)
                    {
                        return "Duplicate thanks note.";
                    }
                    else
                    {                        
                        return "Your thanks note is recorded for this review.";
                    }
                }

            }
            catch (Exception ex)
            {
                return "Something error occured while providing thanks note. Please contact support.";
            }
        }

        public string InsertUpdateClaimSoftwareListing(ClaimApproveRejectListingRequest request)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    var ClaimId = _context.Sp_InsertUpdateClaimSoftwareListing(request.ClaimListingID, request.softwareID, request.userID, request.IsUserVerify, request.Email + request.Domain).FirstOrDefault();
                    if (request.ClaimListingID == 0)
                    {
                        Thread thread = new Thread(() => SendEmailForSoftwareUserVerification(Convert.ToInt32(ClaimId), request.SoftwareName, request.softwareID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true));
                        thread.Start();
                        //SendEmailForSoftwareUserVerification(Convert.ToInt32(ClaimId), request.SoftwareName, request.softwareID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true);
                    }
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        private void SendEmailForSoftwareUserVerification(int userID, string softwareName, int softwareID, string softwareOTP, string workEmail, bool IsClaim)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = workEmail;
            emailProperties.EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com";
            if (!IsClaim)
            {
                emailProperties.EmailSubject = "Company profile verification at upvotes.co";
                emailProperties.EmailBody = GetSoftwareUserVerificationEmailContent(userID, softwareName, softwareID, softwareOTP).ToString();
            }
            else
            {
                emailProperties.EmailSubject = "Company profile claimed at upvotes.co";
                emailProperties.EmailBody = GetSoftwareUserClaimedEmailContent(userID, softwareName, softwareID, workEmail).ToString();
            }

            EmailHelper.SendEmail(emailProperties);
        }

        private string GetSoftwareUserClaimedEmailContent(int userID, string softwareName, int softwareID, string workEmail)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "softwares/claimsoftwareverification?CID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(userID.ToString())) + "&WID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(workEmail)) + "&KID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(softwareID.ToString()));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p>");
            sb.Append("<p>Click the below link to verify your claimed software profile at upvotes.co</p>");
            sb.Append("<p><em><strong><a href = '" + url + "' target = '_blank' rel = 'noopener'>" + url + "</a> &nbsp;</strong></em></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetSoftwareUserVerificationEmailContent(int userID, string softwareName, int softwareID, string softwareOTP)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "UserCompanyList/SoftwareVerificationByUser?UID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(userID.ToString())) + "&CID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(softwareOTP)) + "&KID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(softwareID.ToString()));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p>");
            sb.Append("<p>Click the below link to verify your company profile " + softwareName + " at upvotes.co</p>");
            sb.Append("<p><em><strong><a href = '" + url + "' target = '_blank' rel = 'noopener'>" + url + "</a> &nbsp;</strong></em></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private IEnumerable<SoftwareEntity> GetSoftware(int? serviceCategoryID, string softwareName, int userID, int PageNo, int PageSize, string sortby)
        {
            using (_context = new UpVotesEntities())
            {
                string sqlQuery = "EXEC Sp_GetSoftware " + serviceCategoryID + ",'" + softwareName + "'," + userID + "," + PageNo + "," + PageSize + ",'" + sortby + "'";
                IEnumerable<Sp_GetSoftware_Result> software = _context.Database.SqlQuery(typeof(Sp_GetSoftware_Result), sqlQuery).Cast<Sp_GetSoftware_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSoftware_Result, SoftwareEntity>(); });
                IEnumerable<SoftwareEntity> softwareEntity = Mapper.Map<IEnumerable<Sp_GetSoftware_Result>, IEnumerable<SoftwareEntity>>(software);
                return softwareEntity;
            }
        }

        private IEnumerable<SoftwareReviewsEntity> GetSoftwareReviews(string softwareName, int noOfRows)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetSoftwareReviews_Result> softwareReviews = _context.Database.SqlQuery(typeof(Sp_GetSoftwareReviews_Result), "EXEC Sp_GetSoftwareReviews " + "'" + softwareName + "',"+ noOfRows).Cast<Sp_GetSoftwareReviews_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSoftwareReviews_Result, SoftwareReviewsEntity>(); });
                IEnumerable<SoftwareReviewsEntity> softwareReviewEntity = Mapper.Map<IEnumerable<Sp_GetSoftwareReviews_Result>, IEnumerable<SoftwareReviewsEntity>>(softwareReviews);
                return softwareReviewEntity;
            }
        }

        private IEnumerable<SoftwareReviewThankNoteEntity> GetSoftwareReviewThankNotes(int softwareID, int softwareReviewID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetSoftwareReviewThankNotedUsers_Result> softwareReviewThankNotes = _context.Database.SqlQuery(typeof(Sp_GetSoftwareReviewThankNotedUsers_Result), "EXEC Sp_GetSoftwareReviewThankNotedUsers " + softwareID + "," + softwareReviewID).Cast<Sp_GetSoftwareReviewThankNotedUsers_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSoftwareReviewThankNotedUsers_Result, SoftwareReviewThankNoteEntity>(); });
                IEnumerable<SoftwareReviewThankNoteEntity> softwareReviewThankNotesEntity = Mapper.Map<IEnumerable<Sp_GetSoftwareReviewThankNotedUsers_Result>, IEnumerable<SoftwareReviewThankNoteEntity>>(softwareReviewThankNotes);
                return softwareReviewThankNotesEntity;
            }
        }

        public List<string> GetSoftwareForAutoComplete(int SoftwareCategory, string searchTerm)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<string> myAutoCompleteList = new List<string>();                    
                    using (_context = new UpVotesEntities())
                    {
                        myAutoCompleteList = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetSoftwareNames " + SoftwareCategory + "," + searchTerm).Cast<string>().ToList();
                        return myAutoCompleteList;
                    }                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string VoteForSoftware(SoftwareVoteEntity softwareVote)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    var isVoted = _context.Sp_SoftwareVote(softwareVote.UserID,softwareVote.SoftwareID).FirstOrDefault();
                    
                    if (isVoted == -1)
                    {
                        return "You already voted for this company.";
                    }
                    else
                    {
                        Sp_SoftwareVoteUserInformation_Result user = _context.Sp_SoftwareVoteUserInformation(softwareVote.UserID, softwareVote.SoftwareID).FirstOrDefault();
                        Thread thread = new Thread(() => SendSoftwareEmailForVoting(user, user.SoftwareName));
                        thread.Start();
                        //SendSoftwareEmailForVoting(user, user.SoftwareName);

                        return "Thanks for voting.";
                    }
                }

            }
            catch (Exception ex)
            {
                return "Something error occured while voting. Please contact support.";
                //throw ex;
            }
        }

        private void SendSoftwareEmailForVoting(Sp_SoftwareVoteUserInformation_Result user, string softwareName)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = user.FirstName + " " + user.LastName + " voted for " + softwareName;
            emailProperties.EmailBody = GetSoftwareVotingEmailContent(user, softwareName).ToString();
            EmailHelper.SendEmail(emailProperties);
        }
        private string GetSoftwareVotingEmailContent(Sp_SoftwareVoteUserInformation_Result user, string softwareName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello,</p><p> We got a vote for&nbsp;<em><strong><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "software/" + softwareName.Replace(" ", "-") + "' target = '_blank' rel = 'noopener'>" + softwareName + "</a> &nbsp;</strong></em> from &nbsp;<em><strong>"
                + user.FirstName + " " + user.LastName + ".</strong></em></p>");
            sb.Append("<p> Click <a href = '" + user.ProfileURL + "' target = '_blank' rel = 'noopener' > here </a> to know more about user.</p>");
            sb.Append("<p><a href = '" + user.ProfileURL + "' ><img src = '" + user.ProfilePictureURL + "' alt = '" + user.FirstName + " " + user.LastName + "' width = '80' height = '80' /></a></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        public CategoryMetaTags GetSoftwareCategoryMetaTags(string SoftwareCategoryAreaName)
        {
            CategoryMetaTags metaTagAndTitle = new CategoryMetaTags();
            IEnumerable<CategoryMetaTags> metaTag = SoftwareCategoryMetaTags(SoftwareCategoryAreaName);
            if (metaTag.Count() > 0)
            {
                foreach (CategoryMetaTags metadata in metaTag)
                {
                    metaTagAndTitle.CategoryBasedMetaTagsID = metadata.CategoryBasedMetaTagsID;
                    metaTagAndTitle.FocusAreaName = metadata.FocusAreaName;
                    metaTagAndTitle.SubFocusAreaName = metadata.SubFocusAreaName;
                    metaTagAndTitle.Title = metadata.Title;
                    metaTagAndTitle.TwitterTitle = metadata.TwitterTitle;
                    metaTagAndTitle.Descriptions = metadata.Descriptions;
                }
            }
            return metaTagAndTitle;
        }

        private IEnumerable<CategoryMetaTags> SoftwareCategoryMetaTags(string SoftwareCategoryAreaName)
        {
            using (_context = new UpVotesEntities())
            {
                bool IsService = false;
                IEnumerable<Sp_CategoryMetaTags_Result> metaTag = _context.Database.SqlQuery(typeof(Sp_CategoryMetaTags_Result), "EXEC Sp_CategoryMetaTags '" + SoftwareCategoryAreaName + "','" + 0 + "'," + IsService).Cast<Sp_CategoryMetaTags_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_CategoryMetaTags_Result, CategoryMetaTags>(); });
                IEnumerable<CategoryMetaTags> CategoryMetaTags = Mapper.Map<IEnumerable<Sp_CategoryMetaTags_Result>, IEnumerable<CategoryMetaTags>>(metaTag);
                return CategoryMetaTags;
            }
        }
    }
}
