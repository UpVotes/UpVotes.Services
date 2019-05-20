using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static Logger Log => Logger.Instance();

        private UpVotesEntities _context = null;


        public SoftwareDetail GetAllSoftwareDetails(int? serviceCategoryId, string softwareName, string sortby, int userId = 0, int pageNo = 1, int pageSize = 10)
        {
            SoftwareDetail softwareDetail = new SoftwareDetail();
            OverviewNewsService newsObj = new OverviewNewsService();
            softwareDetail.SoftwareList = new List<SoftwareEntity>();
            softwareName = Helper.BasicDecryptString(softwareName);

            try
            {
                using (_context = new UpVotesEntities())
                {
                    IEnumerable<SoftwareEntity> softwareEntities = GetSoftware(serviceCategoryId, softwareName, userId, pageNo, pageSize, sortby).ToList();
                    if (softwareEntities.Any())
                    {
                        foreach (SoftwareEntity software in softwareEntities)
                        {
                            if (softwareName != "")
                            {
                                software.SoftwareName = System.Web.HttpUtility.HtmlEncode(software.SoftwareName);
                                software.SoftwareReviews = GetSoftwareReviews(software.SoftwareName, 5).ToList();
                                software.OverviewNewsData = newsObj.GetCompanySoftwareNewsByID(2, software.SoftwareID);
                                if (software.SoftwareReviews.Any())
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
            SoftwareDetail softwareDetail = new SoftwareDetail { SoftwareList = new List<SoftwareEntity>() };

            SoftwareEntity software = new SoftwareEntity { SoftwareName = softwareName.Trim() };
            software.SoftwareReviews = GetSoftwareReviews(software.SoftwareName, noOfRows).ToList();

            softwareDetail.SoftwareList.Add(software);

            return softwareDetail;
        }

        public SoftwareDetail GetUserSoftwaresByUserId(int userId, bool isAdmin)
        {
            SoftwareDetail softwareDetail = new SoftwareDetail() { SoftwareList = new List<SoftwareEntity>() };
            using (_context = new UpVotesEntities())
            {
                List<Sp_GetSoftwaresByUserID_Result> dbSoftwareList = new List<Sp_GetSoftwaresByUserID_Result>();
                if (isAdmin)
                {
                    dbSoftwareList = _context.Sp_GetSoftwaresByUserID(userId)
                        .Where(a => a.IsAdminApproved == false).ToList();
                }
                else
                {
                    dbSoftwareList = _context.Sp_GetSoftwaresByUserID(userId).ToList();
                }

                foreach (Sp_GetSoftwaresByUserID_Result softwares in dbSoftwareList)
                {
                    SoftwareEntity softwareEntity = new SoftwareEntity()
                    {
                        SoftwareID = softwares.SoftwareID,
                        SoftwareName = softwares.SoftwareName,
                        IsUserVerified = softwares.IsUserApproved == true ? "Yes" : "No",
                        IsAdminApproved = softwares.IsAdminApproved == true ? "Yes" : "No"
                    };

                    softwareDetail.SoftwareList.Add(softwareEntity);
                }
            }

            return softwareDetail;

        }

        public SoftwareDetail GetUserSoftwareByName(string softwareName)
        {
            SoftwareDetail softwareDetail = new SoftwareDetail() { SoftwareList = new List<SoftwareEntity>() };
            using (_context = new UpVotesEntities())
            {

                List<Sp_GetSoftwareDetailsByName_Result> dbSoftwareList = _context.Sp_GetSoftwareDetailsByName(softwareName).ToList();

                foreach (Sp_GetSoftwareDetailsByName_Result softwares in dbSoftwareList)
                {
                    SoftwareEntity softwareEntity = new SoftwareEntity()
                    {
                        SoftwareID = softwares.SoftwareID,
                        SoftwareName = softwares.SoftwareName,
                        LogoName = softwares.LogoName,
                        TagLine = softwares.TagLine,
                        PriceRange = softwares.PriceRange,
                        SoftwareTrail = softwares.SoftwareTrail,
                        WebSiteURL = softwares.WebSiteURL,
                        FoundedYear = softwares.FoundedYear,
                        DemoURL = softwares.DemoURL,
                        SoftwareDescription = softwares.SoftwareDescription,
                        WorkEmail = softwares.WorkEmail,
                        DomainID = softwares.DomainID,
                        LinkedInURL = softwares.LinkedInURL,
                        FaceBookURL = softwares.FaceBookURL,
                        TwitterURL = softwares.TwitterURL,
                        InstagramURL = softwares.InstagramURL,
                        Remarks = softwares.Remarks,
                        CreatedBy = softwares.CreatedBy,
                        SoftwareCatagoryIds = _context.Sp_GetSoftwareCategoryIds(softwares.SoftwareID).FirstOrDefault(),
                    };

                    softwareDetail.SoftwareList.Add(softwareEntity);
                }
            }

            return softwareDetail;
        }

        public int SaveSoftwareDetails(SoftwareEntity softwareEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Softwares softwareObj = null;
                    int softwareId = 0;
                    bool isAdmin = false;
                    string softwareOtp = string.Empty; bool isAdd = false;
                    Sp_CheckForSoftwareAndAdminUser_Result checkForSoftwareAndAdminUserObj = _context.Sp_CheckForSoftwareAndAdminUser(softwareEntity.SoftwareName, softwareEntity.LoggedInUserName).FirstOrDefault();

                    if (checkForSoftwareAndAdminUserObj != null)
                    {
                        isAdmin = Convert.ToBoolean(checkForSoftwareAndAdminUserObj.IsAdmin);
                        if (softwareEntity.SoftwareID == 0)
                        {
                            if (Convert.ToBoolean(checkForSoftwareAndAdminUserObj.IsSoftwareExists))
                            {
                                return 0;
                            }
                            isAdd = true;
                            softwareId = Convert.ToInt32(checkForSoftwareAndAdminUserObj.PreviousSoftwareID);
                            softwareId = softwareId + 1;
                            softwareEntity.SoftwareID = softwareId;

                            softwareObj = new Softwares();
                            SoftwareDataInitialization(softwareEntity, softwareObj, isAdmin);
                            softwareObj.CreatedBy = softwareEntity.CreatedBy;
                            softwareObj.CreatedDate = DateTime.Now;
                            softwareObj.SoftwareOTP = new Random().Next(100000, 999999).ToString("D6");
                            softwareObj.IsActive = true;
                            softwareObj.IsAdminApproved = false;
                            _context.Softwares.Add(softwareObj);
                        }
                        else
                        {
                            isAdd = false;
                            softwareObj = _context.Softwares.FirstOrDefault(s => s.SoftwareID == softwareEntity.SoftwareID);
                            SoftwareDataInitialization(softwareEntity, softwareObj, checkForSoftwareAndAdminUserObj.IsAdmin);
                            softwareObj.ModifiedBy = softwareEntity.ModifiedBy;
                            softwareObj.ModifiedDate = DateTime.Now;

                            if (!isAdmin && softwareObj.IsUserApproved == true && softwareObj.IsAdminApproved == true)
                            {
                                _context.SP_CopySoftware(softwareEntity.SoftwareID);
                                softwareObj.IsAdminApproved = false;
                                softwareObj.AdminApprovedDate = null;
                                SendSoftwareEditNotificationToAdmin(softwareObj.SoftwareName);                                
                            }
                        }
                    }

                    _context.SaveChanges();

                    SaveSoftwareCategory(softwareEntity, _context);

                    if (isAdd)
                    {
                        if (!isAdmin)
                        {
                            SendEmailForSoftwareUserVerification(softwareEntity.LoggedInUserName,
                                softwareObj.SoftwareName, softwareEntity.SoftwareID, softwareObj.SoftwareOTP,
                                softwareObj.WorkEmail, false);
                        }                        
                    }

                    if (!isAdmin)
                    {
                        _context.Sp_InsertSoftwarePendingForApproval(softwareEntity.SoftwareID);
                    }
                    else
                    {
                        if (!isAdd && Convert.ToBoolean(softwareObj.IsUserApproved) && !string.IsNullOrEmpty(softwareObj.WorkEmail))
                        {                            
                            _context.Sp_DeleteSoftwarePendingForApproval(softwareEntity.SoftwareID);
                            if (!Convert.ToBoolean(softwareObj.IsAdminApproved))
                            {
                                User newUserObj = AddUserByWorkEmailID(softwareObj, _context);                                
                                SendSoftwareApprovedEmail(softwareObj.SoftwareName, softwareObj.WorkEmail, newUserObj);                                
                                softwareObj.CreatedBy = newUserObj.UserID;
                            }
                            softwareObj.IsAdminApproved = true;
                            softwareObj.AdminApprovedDate = DateTime.Now;

                            _context.Sp_DeleteSoftwareHistory(softwareObj.SoftwareID);
                        }
                    }

                    _context.SaveChanges();

                    return softwareId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendSoftwareEditNotificationToAdmin(string softwareName)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = softwareName + " is updated by user.",
                EmailBody = "FYI...",
            };

            EmailHelper.SendEmail(emailProperties);
        }

        private void SaveSoftwareCategory(SoftwareEntity softwareEntity, UpVotesEntities context)
        {
            _context.Sp_DeleteSoftwareCatagory(softwareEntity.SoftwareID);

            foreach (string categoryId in softwareEntity.SoftwareCatagoryIds.Split('_'))
            {
                _context.Sp_InsSoftwareCategory(softwareEntity.SoftwareID, Convert.ToInt32(categoryId),
                    softwareEntity.LoggedInUserName);                
                //_context.SaveChanges();
            }            
        }

        public bool SoftwareVerificationByUser(int uId, string cId, int softId)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Softwares userVerifiedSoftware = _context.Softwares.FirstOrDefault(a => a.SoftwareID == softId && a.CreatedBy == uId && a.SoftwareOTP.Trim().ToUpper() == cId.Trim().ToUpper());
                    if (userVerifiedSoftware != null)
                    {
                        userVerifiedSoftware.IsUserApproved = true;
                        userVerifiedSoftware.UserApprovedDate = DateTime.Now;
                        _context.SaveChanges();
                        SendUserApprovedEmail(userVerifiedSoftware.SoftwareName);                        
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSoftwareRejectionComments(SoftwareRejectComments softwareRejectComments)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    _context.Sp_UpdSoftwareRejectComments(softwareRejectComments.SoftwareId,
                        softwareRejectComments.RejectedBy, softwareRejectComments.RejectComments,
                        DateTime.Now.ToLongDateString());                    
                    //_context.SaveChanges();
                    SendSoftwareRejectedEmail(softwareRejectComments.SoftwareName, softwareRejectComments.WorkEmail, softwareRejectComments.RejectComments);

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SendSoftwareRejectedEmail(string softwareName, string workEmail, string reason)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = workEmail,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = softwareName + " is rejected at upvotes.co",
                EmailBody = GetAdminRejectedEmailContent(softwareName, reason)
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAdminRejectedEmailContent(string softwareName, string reason)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p> Your software profile " + softwareName + " has been rejected from admin at upvotes.co due to the below reason </p>");
            sb.Append("<p><strong>" + reason + "</strong></p>");
            sb.Append("<p>Please correct to get it listed at upvotes.co</p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private void SendUserApprovedEmail(string softwareName)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = softwareName + " is approved by user.",
                EmailBody = GetUserApprovedEmailContent(softwareName)
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetUserApprovedEmailContent(string softwareName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p>Hello Admin, <br/> <b>" + softwareName + "<b/> has been approved from User.</p>");

            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private User AddUserByWorkEmailID(Softwares softwareObj, UpVotesEntities _upvotesContext)
        {
            User dbUser = _upvotesContext.Users.FirstOrDefault(a => a.UserName.Trim().ToUpper() == softwareObj.WorkEmail.Trim().ToUpper());

            if (dbUser == null)
            {
                dbUser = _upvotesContext.Users.FirstOrDefault(a => a.UserID == softwareObj.CreatedBy);
                if (dbUser != null)
                {
                    User dbNewUser = new User()
                    {
                        UserName = softwareObj.WorkEmail.Trim(),
                        UserPassword = EncryptionAndDecryption.Encrypt((dbUser.FirstName ?? "!Upvotes") + EncryptionAndDecryption.GenRandomAlphaNum(6)),
                        FirstName = dbUser.FirstName,
                        LastName = dbUser.LastName,
                        UserEmail = dbUser.UserEmail,
                        UserMobile = dbUser.UserMobile,
                        UserType = 1,
                        ProfileURL = dbUser.ProfileURL,
                        IsActive = true,
                        IsBlocked = false,
                        UserActivatedDateTime = DateTime.Now,
                        UserLastLoginDateTime = DateTime.Now,
                        Remarks = dbUser.Remarks,
                        DateOfBirth = dbUser.DateOfBirth,
                        ProfilePictureURL = dbUser.ProfilePictureURL,
                        ProfileID = dbUser.ProfileID,
                        CreatedBy = dbUser.UserID,
                        CreatedDate = DateTime.Now
                    };
                    _upvotesContext.Users.Add(dbNewUser);
                    _upvotesContext.SaveChanges();
                }
            }

            dbUser = _upvotesContext.Users.FirstOrDefault(a => a.UserName.Trim().ToUpper() == softwareObj.WorkEmail.Trim().ToUpper());

            softwareObj.CreatedBy = dbUser.UserID;

            return dbUser;
        }

        private void SoftwareDataInitialization(SoftwareEntity softwareEntity, Softwares softwareObj, bool? isAdmin)
        {
            softwareObj.SoftwareID = softwareEntity.SoftwareID;
            softwareObj.SoftwareName = softwareEntity.SoftwareName;
            softwareObj.TagLine = softwareEntity.TagLine;
            if (!string.IsNullOrWhiteSpace(softwareEntity.LogoName))
            {
                softwareObj.LogoName = softwareEntity.LogoName;
            }

            softwareObj.PriceRange = softwareEntity.PriceRange;
            softwareObj.SoftwareTrail = softwareEntity.SoftwareTrail;
            softwareObj.DemoURL = softwareEntity.DemoURL;
            if (Convert.ToBoolean(isAdmin) && softwareEntity.CreatedBy == softwareEntity.LoggedInUserName)
            {
                softwareObj.DomainID = softwareEntity.DomainID;
            }
            else
            {
                softwareObj.WorkEmail = softwareEntity.WorkEmail;
                softwareObj.DomainID = string.Empty;
            }

            softwareObj.WebSiteURL = softwareEntity.WebSiteURL;
            softwareObj.LinkedInURL = softwareEntity.LinkedInURL;
            softwareObj.FaceBookURL = softwareEntity.FaceBookURL;
            softwareObj.TwitterURL = softwareEntity.TwitterURL;
            softwareObj.InstagramURL = softwareEntity.InstagramURL;
            softwareObj.FoundedYear = softwareEntity.FoundedYear;
            softwareObj.SoftwareDescription = softwareEntity.SoftwareDescription;
            softwareObj.Remarks = softwareEntity.Remarks;
        }

        public CompanySoftwareReviews GetReviewsForSoftwareListingPage(SoftwareFilterEntity softwareFilter)
        {
            CompanySoftwareReviews softwareReviewsObj = new CompanySoftwareReviews
            {
                SoftwareReviewsList = new List<SoftwareReviewsEntity>(),
                SoftwareNamesList = new List<string>()
            };


            using (_context = new UpVotesEntities())
            {
                List<Sp_GetSoftwareReviewsForListingPage_Result> response = _context.Sp_GetSoftwareReviewsForListingPage(softwareFilter.SoftwareName, softwareFilter.SoftwareCategoryId, 5, softwareFilter.PageNo, softwareFilter.PageSize).ToList();
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

                    decimal? isThankNote = _context.Sp_InsSoftwareReviewForThanksNote(softwareReviewThanksNoteEntity.UserID, softwareReviewThanksNoteEntity.SoftwareReviewID).FirstOrDefault();

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
            catch (Exception)
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
                    int? ClaimId = _context.Sp_InsertUpdateClaimSoftwareListing(request.ClaimListingID, request.softwareID, request.userID, request.IsUserVerify, request.Email + request.Domain).FirstOrDefault();
                    if (request.ClaimListingID == 0)
                    {
                        Thread thread = new Thread(() => SendEmailForSoftwareUserVerification(Convert.ToInt32(ClaimId), request.SoftwareName, request.softwareID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true));
                        thread.Start();
                        //SendEmailForSoftwareUserVerification(Convert.ToInt32(ClaimId), request.SoftwareName, request.softwareID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true);
                    }
                    return "OK";
                }
            }
            catch (Exception)
            {
                return "Error";
            }
        }

        private void SendEmailForSoftwareUserVerification(int userID, string softwareName, int softwareID, string softwareOTP, string workEmail, bool IsClaim)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = workEmail,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com"
            };
            if (!IsClaim)
            {
                emailProperties.EmailSubject = "Software profile verification at upvotes.co";
                emailProperties.EmailBody = GetSoftwareUserVerificationEmailContent(userID, softwareName, softwareID, softwareOTP).ToString();
            }
            else
            {
                emailProperties.EmailSubject = "Software profile claimed at upvotes.co";
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
            sb.Append("<p>Click the below link to verify your software profile " + softwareName + " at upvotes.co</p>");
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
                IEnumerable<Sp_GetSoftwareReviews_Result> softwareReviews = _context.Database.SqlQuery(typeof(Sp_GetSoftwareReviews_Result), "EXEC Sp_GetSoftwareReviews " + "'" + softwareName + "'," + noOfRows).Cast<Sp_GetSoftwareReviews_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSoftwareReviews_Result, SoftwareReviewsEntity>(); });
                IEnumerable<SoftwareReviewsEntity> softwareReviewEntity = Mapper.Map<IEnumerable<Sp_GetSoftwareReviews_Result>, IEnumerable<SoftwareReviewsEntity>>(softwareReviews);
                return softwareReviewEntity;
            }
        }

        private IEnumerable<SoftwareReviewThankNoteEntity> GetSoftwareReviewThankNotes(int softwareId, int softwareReviewId)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetSoftwareReviewThankNotedUsers_Result> softwareReviewThankNotes = _context.Database.SqlQuery(typeof(Sp_GetSoftwareReviewThankNotedUsers_Result), "EXEC Sp_GetSoftwareReviewThankNotedUsers " + softwareId + "," + softwareReviewId).Cast<Sp_GetSoftwareReviewThankNotedUsers_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSoftwareReviewThankNotedUsers_Result, SoftwareReviewThankNoteEntity>(); });
                IEnumerable<SoftwareReviewThankNoteEntity> softwareReviewThankNotesEntity = Mapper.Map<IEnumerable<Sp_GetSoftwareReviewThankNotedUsers_Result>, IEnumerable<SoftwareReviewThankNoteEntity>>(softwareReviewThankNotes);
                return softwareReviewThankNotesEntity;
            }
        }

        public List<string> GetSoftwareForAutoComplete(int softwareCategory, string searchTerm)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<string> myAutoCompleteList = new List<string>();
                    using (_context = new UpVotesEntities())
                    {
                        myAutoCompleteList = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetSoftwareNames " + softwareCategory + "," + searchTerm).Cast<string>().ToList();
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
                    decimal? isVoted = _context.Sp_SoftwareVote(softwareVote.UserID, softwareVote.SoftwareID).FirstOrDefault();

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
            catch (Exception)
            {
                return "Something error occured while voting. Please contact support.";
                //throw ex;
            }
        }

        private void SendSoftwareEmailForVoting(Sp_SoftwareVoteUserInformation_Result user, string softwareName)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"],
                EmailBCC = "support@upvotes.co; puneethm@hotmail.com",
                EmailSubject = user.FirstName + " " + user.LastName + " voted for " + softwareName,
                EmailBody = GetSoftwareVotingEmailContent(user, softwareName).ToString()
            };
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

        public CategoryMetaTags GetSoftwareCategoryMetaTags(string softwareCategoryAreaName)
        {
            CategoryMetaTags metaTagAndTitle = new CategoryMetaTags();
            IEnumerable<CategoryMetaTags> metaTag = SoftwareCategoryMetaTags(softwareCategoryAreaName);
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

        private IEnumerable<CategoryMetaTags> SoftwareCategoryMetaTags(string softwareCategoryAreaName)
        {
            using (_context = new UpVotesEntities())
            {
                bool IsService = false;
                IEnumerable<Sp_CategoryMetaTags_Result> metaTag = _context.Database.SqlQuery(typeof(Sp_CategoryMetaTags_Result), "EXEC Sp_CategoryMetaTags '" + softwareCategoryAreaName + "','" + 0 + "'," + IsService).Cast<Sp_CategoryMetaTags_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_CategoryMetaTags_Result, CategoryMetaTags>(); });
                IEnumerable<CategoryMetaTags> CategoryMetaTags = Mapper.Map<IEnumerable<Sp_CategoryMetaTags_Result>, IEnumerable<CategoryMetaTags>>(metaTag);
                return CategoryMetaTags;
            }
        }

        private void SendSoftwareApprovedEmail(string softwareName, string workEmail, User newUserObj)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = workEmail,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = softwareName + " is updated at upvotes.co",
                EmailBody = GetAdminApprovedEmailContent(softwareName, newUserObj)
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAdminApprovedEmailContent(string softwareName, User newUserObj)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p> Your software profile " + softwareName + " has been updated at upvotes.co. Click on the below link to verify the contents.</p>");
            sb.Append("<p><em><strong><a href='" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "software/" + softwareName.Replace(" ", "-").Trim().ToLower() + "' target='_blank' rel='noopener'>" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "software/" + softwareName.Replace(" ", "-").Trim().ToLower() + "</a></strong></em><p>");
            if (newUserObj != null)
            {
                sb.Append("<p>Please use below credentials to login to the upvotes portal.</p>");
                sb.Append("<p> User Name:<br/>" + newUserObj.UserName + "</p>");
                sb.Append("<p> Password:<br/>" + EncryptionAndDecryption.Decrypt(newUserObj.UserPassword) + "</p>");
            }

            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }
    }
}
