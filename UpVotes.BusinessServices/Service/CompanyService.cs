using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessEntities.Helper;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class CompanyService : ICompanyService
    {
        private static Logger Log
        {
            get
            {
                return Logger.Instance();
            }
        }

        UpVotesEntities _context = null;

        public int SaveCompany(CompanyEntity companyEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Company companyObj = null; int companyID = 0;
                    bool isAdd = false; string companyOTP = string.Empty;
                    if (companyEntity.CompanyID == 0)
                    {
                        isAdd = true;
                        companyID = _context.Company.OrderByDescending(a => a.CompanyID).Take(1).Select(a => a.CompanyID).FirstOrDefault();
                        companyID = companyID + 1;
                        companyEntity.CompanyID = companyID;

                        companyObj = new Company();
                        CompanyInitialization(companyEntity, companyObj);
                        companyObj.CreatedBy = companyEntity.UserID;
                        companyObj.CreatedDate = DateTime.Now;
                        companyObj.CompanyOTP = new Random().Next(100000, 999999).ToString("D6");
                        companyObj.IsActive = true;
                        _context.Company.Add(companyObj);
                    }
                    else
                    {
                        isAdd = false;
                        companyObj = _context.Company.Where(c => c.CompanyID == companyEntity.CompanyID).FirstOrDefault();
                        CompanyInitialization(companyEntity, companyObj);
                        companyObj.ModifiedBy = companyEntity.UserID;
                        companyObj.ModifiedDate = DateTime.Now;
                        companyID = companyEntity.CompanyID;
                    }

                    _context.SaveChanges();

                    if (companyEntity.CompanyFocus.Any())
                    {
                        CompanyFocus companyFocusObj = null;
                        string deleteFocusQuery = "DELETE A FROM dbo.CompanySubFocus A JOIN dbo.CompanyFocus B ON A.CompanyFocusID = B.CompanyFocusID WHERE B.CompanyID =" + companyEntity.CompanyID + "  DELETE FROM dbo.CompanyFocus WHERE CompanyID=" + companyEntity.CompanyID;
                        _context.Database.ExecuteSqlCommand(deleteFocusQuery);

                        int companyFocusID = _context.CompanyFocus.OrderByDescending(a => a.CompanyFocusID).Select(a => a.CompanyFocusID).FirstOrDefault();
                        int i = 0;
                        foreach (CompanyFocusEntity companyFocusEntityObj in companyEntity.CompanyFocus)
                        {
                            i = i + 1;
                            companyFocusObj = new CompanyFocus();
                            companyFocusObj.CompanyID = companyEntity.CompanyID;
                            companyFocusObj.CompanyFocusID = companyFocusID + i;
                            companyFocusObj.FocusAreaID = companyFocusEntityObj.FocusAreaID;
                            companyFocusObj.FocusAreaPercentage = companyFocusEntityObj.FocusAreaPercentage;
                            companyFocusObj.IsActive = true;
                            companyFocusObj.CreatedBy = companyEntity.UserID;
                            companyFocusObj.CreatedDate = DateTime.Now;
                            _context.CompanyFocus.Add(companyFocusObj);
                            //_context.SaveChanges();

                            CompanySubFocus dbCompanySubFocusObj = null;
                            foreach (CompanySubFocusEntity companySubFocusEntityObj in companyFocusEntityObj.CompanySubFocus)
                            {
                                dbCompanySubFocusObj = new CompanySubFocus();
                                dbCompanySubFocusObj.CompanyFocusID = companyFocusID + i;
                                dbCompanySubFocusObj.SubFocusAreaID = companySubFocusEntityObj.SubFocusAreaID;
                                dbCompanySubFocusObj.SubFocusAreaPercentage = companySubFocusEntityObj.SubFocusAreaPercentage;
                                dbCompanySubFocusObj.IsActive = true;
                                dbCompanySubFocusObj.CreatedBy = companyEntity.UserID;
                                dbCompanySubFocusObj.CreatedDate = DateTime.Now;
                                _context.CompanySubFocus.Add(dbCompanySubFocusObj);
                                // _context.SaveChanges();
                            }
                        }

                        _context.SaveChanges();
                    }

                    if (companyEntity.CompanyBranches.Any())
                    {
                        CompanyBranch companyBranchObj = null;
                        string deleteBranchQuery = "DELETE FROM dbo.CompanyBranch WHERE CompanyID = " + companyEntity.CompanyID;
                        _context.Database.ExecuteSqlCommand(deleteBranchQuery);

                        int companyBranchID = _context.CompanyBranches.OrderByDescending(a => a.BranchID).Select(a => a.BranchID).FirstOrDefault();
                        int j = 0;
                        foreach (CompanyBranchEntity companyBranchEntityObj in companyEntity.CompanyBranches)
                        {
                            j = j + 1;
                            companyBranchObj = new CompanyBranch();
                            companyBranchObj.BranchID = companyBranchID + j;
                            companyBranchObj.BranchName = companyBranchEntityObj.BranchName;
                            companyBranchObj.CompanyID = companyEntity.CompanyID;
                            companyBranchObj.CountryID = companyBranchEntityObj.CountryID;
                            companyBranchObj.StateID = companyBranchEntityObj.StateID;
                            companyBranchObj.Address = companyBranchEntityObj.Address;
                            companyBranchObj.City = companyBranchEntityObj.City;
                            companyBranchObj.PostalCode = companyBranchEntityObj.PostalCode;
                            companyBranchObj.PhoneNumber = companyBranchEntityObj.PhoneNumber;
                            companyBranchObj.Email = companyBranchEntityObj.Email;
                            companyBranchObj.IsHeadQuarters = companyBranchEntityObj.IsHeadQuarters;
                            companyBranchObj.IsActive = true;
                            companyBranchObj.CreatedBy = companyEntity.UserID;
                            companyBranchObj.CreatedDate = DateTime.Now;
                            _context.CompanyBranches.Add(companyBranchObj);
                        }

                        _context.SaveChanges();
                    }

                    if (isAdd)
                    {                        
                        SendEmailForUserVerification(companyEntity.UserID, companyObj.CompanyName, companyID, companyObj.CompanyOTP, companyObj.WorkEmail);
                    }

                    if (!companyEntity.IsAdminUser)
                    {
                        string insertQuery = "IF NOT EXISTS (SELECT * FROM dbo.CompanyPendingForApproval WHERE CompanyID = " + companyEntity.CompanyID + ") BEGIN INSERT INTO CompanyPendingForApproval (CompanyID) VALUES (" + companyEntity.CompanyID + ") END";
                        _context.Database.ExecuteSqlCommand(insertQuery, companyID);
                        companyObj.IsAdminApproved = false;
                    }
                    else
                    {
                        string deleteQuery = "DELETE FROM dbo.CompanyPendingForApproval WHERE CompanyID =" + companyEntity.CompanyID;
                        _context.Database.ExecuteSqlCommand(deleteQuery, companyID);
                        companyObj.IsAdminApproved = true;
                        companyObj.AdminApprovedDate = DateTime.Now;
                        User newUserObj = AddUserByWorkEmailID(companyObj, _context);
                        SendCompanyApprovedEmail(companyObj.CompanyName, companyObj.WorkEmail, newUserObj);
                    }

                    _context.SaveChanges();

                    return companyID;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private User AddUserByWorkEmailID(Company companyObj, UpVotesEntities _upvotesContext)
        {
            bool isUserExists = _upvotesContext.Users.Where(a => a.UserName.Trim().ToUpper() == companyObj.WorkEmail.Trim().ToUpper()).FirstOrDefault() == null ? false : true;
            if (!isUserExists)
            {
                User dbUser = _upvotesContext.Users.Where(a => a.UserID == companyObj.CreatedBy).FirstOrDefault();
                if (dbUser != null)
                {
                    User newUserObj = new User()
                    {
                        UserName = companyObj.WorkEmail,
                        UserPassword = "$upVotes007@!",
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
                    _upvotesContext.Users.Add(newUserObj);
                    _upvotesContext.SaveChanges();

                    return newUserObj;
                }
            }

            return null;
        }

        private void CompanyInitialization(CompanyEntity companyEntity, Company companyObj)
        {
            companyObj.CompanyID = companyEntity.CompanyID;
            companyObj.CompanyName = companyEntity.CompanyName;
            if (companyEntity.LogoName != string.Empty && companyEntity.LogoName != null)
            {
                companyObj.LogoName = companyEntity.LogoName;
            }
            companyObj.TagLine = companyEntity.TagLine;
            companyObj.FoundedYear = companyEntity.FoundedYear;
            companyObj.MinEmployeeCount = Convert.ToInt32(companyEntity.TotalEmployees.Split('-')[0]);
            companyObj.MaxEmployeeCount = Convert.ToInt32(companyEntity.TotalEmployees.Split('-')[1]);
            companyObj.MinHourleyRate = Convert.ToDecimal(companyEntity.AveragHourlyRate.Replace("$", "").Split('-')[0].Trim());
            companyObj.MaxHourleyRate = Convert.ToDecimal(companyEntity.AveragHourlyRate.Replace("$", "").Split('-')[1].Trim());
            companyObj.URL = companyEntity.URL;
            companyObj.LinkedInProfileURL = companyEntity.LinkedInProfileURL;
            companyObj.TwitterProfileURL = companyEntity.TwitterProfileURL;
            companyObj.FacebookProfileURL = companyEntity.FacebookProfileURL;
            companyObj.GooglePlusProfileURL = companyEntity.GooglePlusProfileURL;
            companyObj.Summary = companyEntity.Summary;
            companyObj.KeyClients = companyEntity.KeyClients;
            companyObj.WorkEmail = companyEntity.WorkEmail;
        }

        public bool DeleteCompany(int companyID)
        {
            throw new NotImplementedException();
        }

        public CategoryMetaTags GetCategoryMetaTags(string FocusAreaName, string SubFocusAreaName)
        {
            CategoryMetaTags metaTagAndTitle = new CategoryMetaTags();
            IEnumerable<CategoryMetaTags> metaTag = CategoryMetaTags(FocusAreaName, SubFocusAreaName);
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

        public CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, string subFocusArea = "0", int userID = 0, int PageNo = 1, int PageSize = 10)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();
            companyName = Helper.BasicDecryptString(companyName);

            try
            {
                IEnumerable<CompanyEntity> companyEntities = GetCompany(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID, location, userID, PageNo, PageSize, subFocusArea);
                if (companyEntities.Count() > 0)
                {
                    foreach (CompanyEntity company in companyEntities)
                    {
                        if (companyName != "0")
                        {
                            company.CompanyName = System.Web.HttpUtility.HtmlEncode(company.CompanyName);
                            company.CompanyFocus = GetCompanyFocus(company.CompanyID).ToList();
                            company.IndustialCompanyFocus = GetIndustrialFocus(company.CompanyID).ToList();
                            company.CompanyClientFocus = GetClientFocus(company.CompanyID).ToList();
                            company.SubfocusNames = GetDistinctSubFocusNames(company.CompanyID).ToList();
                            company.CompanySubFocus = GetCompanySubFocus(company.CompanyID).ToList();
                            company.CompanyBranches = GetCompanyBranches(company.CompanyID).ToList();
                            company.CompanyPortFolio = GetCompanyPortFolio(company.CompanyID).ToList();
                            company.CompanyReviews = GetCompanyReviews(company.CompanyName).ToList();
                            if (company.CompanyReviews.Count() > 0)
                            {
                                foreach (CompanyReviewsEntity companyReviewsEntity in company.CompanyReviews)
                                {
                                    companyReviewsEntity.NoOfThankNotes = GetCompanyReviewThankNotes(company.CompanyID, companyReviewsEntity.CompanyReviewID).Count();
                                }
                            }
                        }

                        companyDetail.CompanyList.Add(company);
                    }
                }

                return companyDetail;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<CompanyEntity> GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, int userID = 0, int PageNo = 1, int PageSize = 10, string subFocusArea = "0")
        {
            using (_context = new UpVotesEntities())
            {
                string sqlQuery = "EXEC Sp_GetCompany '" + companyName + "'," + minRate + "," + maxRate + "," + minEmployee + "," + maxEmployee + ",'" + sortby + "'," + focusAreaID + "," + userID + "," + location + "," + PageNo + "," + PageSize + ",'" + subFocusArea + "'";
                IEnumerable<Sp_GetCompany_Result> company = _context.Database.SqlQuery(typeof(Sp_GetCompany_Result), sqlQuery).Cast<Sp_GetCompany_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompany_Result, CompanyEntity>(); });
                IEnumerable<CompanyEntity> companyEntity = Mapper.Map<IEnumerable<Sp_GetCompany_Result>, IEnumerable<CompanyEntity>>(company);
                return companyEntity;
            }
        }

        private IEnumerable<CompanyPortFolioEntity> GetCompanyPortFolio(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyPortFolio_Result> companyPortFolio = _context.Database.SqlQuery(typeof(Sp_GetCompanyPortFolio_Result), "EXEC Sp_GetCompanyPortFolio " + companyID).Cast<Sp_GetCompanyPortFolio_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyPortFolio_Result, CompanyPortFolioEntity>(); });
                IEnumerable<CompanyPortFolioEntity> companyPortFolioEntity = Mapper.Map<IEnumerable<Sp_GetCompanyPortFolio_Result>, IEnumerable<CompanyPortFolioEntity>>(companyPortFolio);
                return companyPortFolioEntity;
            }
        }

        private IEnumerable<CompanyFocusEntity> GetCompanyFocus(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyFocus_Result> companyFocus = _context.Database.SqlQuery(typeof(Sp_GetCompanyFocus_Result), "EXEC Sp_GetCompanyFocus " + companyID).Cast<Sp_GetCompanyFocus_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyFocus_Result, CompanyFocusEntity>(); });
                IEnumerable<CompanyFocusEntity> companyFocusEntity = Mapper.Map<IEnumerable<Sp_GetCompanyFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyFocus);
                return companyFocusEntity;
            }
        }

        private IEnumerable<CategoryMetaTags> CategoryMetaTags(string FocusAreaName, string SubFocusAreaName)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_CategoryMetaTags_Result> metaTag = _context.Database.SqlQuery(typeof(Sp_CategoryMetaTags_Result), "EXEC Sp_CategoryMetaTags '" + FocusAreaName + "','" + SubFocusAreaName + "'").Cast<Sp_CategoryMetaTags_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_CategoryMetaTags_Result, CategoryMetaTags>(); });
                IEnumerable<CategoryMetaTags> CategoryMetaTags = Mapper.Map<IEnumerable<Sp_CategoryMetaTags_Result>, IEnumerable<CategoryMetaTags>>(metaTag);
                return CategoryMetaTags;
            }
        }

        private IEnumerable<CompanyFocusEntity> GetIndustrialFocus(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetIndustrialFocus_Result> companyIndustrialFocus = _context.Database.SqlQuery(typeof(Sp_GetIndustrialFocus_Result), "EXEC Sp_GetIndustrialFocus " + companyID).Cast<Sp_GetIndustrialFocus_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetIndustrialFocus_Result, CompanyFocusEntity>(); });
                IEnumerable<CompanyFocusEntity> companyFocusEntity = Mapper.Map<IEnumerable<Sp_GetIndustrialFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyIndustrialFocus);
                return companyFocusEntity;
            }
        }

        private IEnumerable<CompanyFocusEntity> GetClientFocus(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetClientFocus_Result> companyclientFocus = _context.Database.SqlQuery(typeof(Sp_GetClientFocus_Result), "EXEC Sp_GetClientFocus " + companyID).Cast<Sp_GetClientFocus_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetClientFocus_Result, CompanyFocusEntity>(); });
                IEnumerable<CompanyFocusEntity> companyclientFocusEntity = Mapper.Map<IEnumerable<Sp_GetClientFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyclientFocus);
                return companyclientFocusEntity;
            }
        }

        private IEnumerable<string> GetDistinctSubFocusNames(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<string> SubFocusNames = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetDistinctSubFocusNames " + companyID).Cast<string>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<string, CompanyFocusEntity>(); });
                IEnumerable<string> SubfocusNamesEntity = Mapper.Map<IEnumerable<string>, IEnumerable<string>>(SubFocusNames);
                return SubfocusNamesEntity;
            }
        }

        private IEnumerable<CompanyFocusEntity> GetCompanySubFocus(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetSubFocus_Result> companyclientFocus = _context.Database.SqlQuery(typeof(Sp_GetSubFocus_Result), "EXEC Sp_GetSubFocus " + companyID).Cast<Sp_GetSubFocus_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetSubFocus_Result, CompanyFocusEntity>(); });
                IEnumerable<CompanyFocusEntity> companyclientFocusEntity = Mapper.Map<IEnumerable<Sp_GetSubFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyclientFocus);
                return companyclientFocusEntity;
            }
        }

        private IEnumerable<CompanyBranchEntity> GetCompanyBranches(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyBranches_Result> companyBranches = _context.Database.SqlQuery(typeof(Sp_GetCompanyBranches_Result), "EXEC Sp_GetCompanyBranches " + companyID).Cast<Sp_GetCompanyBranches_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyBranches_Result, CompanyBranchEntity>(); });
                IEnumerable<CompanyBranchEntity> companyBranchEntity = Mapper.Map<IEnumerable<Sp_GetCompanyBranches_Result>, IEnumerable<CompanyBranchEntity>>(companyBranches);
                return companyBranchEntity;
            }
        }

        private IEnumerable<CompanyReviewsEntity> GetCompanyReviews(string companyName)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyReviews_Result> companyReviews = _context.Database.SqlQuery(typeof(Sp_GetCompanyReviews_Result), "EXEC Sp_GetCompanyReviews " + "'" + companyName + "'").Cast<Sp_GetCompanyReviews_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyReviews_Result, CompanyReviewsEntity>(); });
                IEnumerable<CompanyReviewsEntity> companyReviewEntity = Mapper.Map<IEnumerable<Sp_GetCompanyReviews_Result>, IEnumerable<CompanyReviewsEntity>>(companyReviews);
                return companyReviewEntity;
            }
        }

        private IEnumerable<CompanyReviewThankNoteEntity> GetCompanyReviewThankNotes(int companyID, int companyReviewID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyReviewThankNotedUsers_Result> companyReviewThankNotes = _context.Database.SqlQuery(typeof(Sp_GetCompanyReviewThankNotedUsers_Result), "EXEC Sp_GetCompanyReviewThankNotedUsers " + companyID + "," + companyReviewID).Cast<Sp_GetCompanyReviewThankNotedUsers_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyReviewThankNotedUsers_Result, CompanyReviewThankNoteEntity>(); });
                IEnumerable<CompanyReviewThankNoteEntity> companyReviewThankNotesEntity = Mapper.Map<IEnumerable<Sp_GetCompanyReviewThankNotedUsers_Result>, IEnumerable<CompanyReviewThankNoteEntity>>(companyReviewThankNotes);
                return companyReviewThankNotesEntity;
            }
        }

        public string VoteForCompany(CompanyVoteEntity companyVote)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    bool isVoted = _context.CompanyVotes.Where(e => e.CompanyID == companyVote.CompanyID && e.UserID == companyVote.UserID).Count() > 0 ? true : false;

                    if (isVoted)
                    {
                        return "You already voted for this company.";
                    }
                    else
                    {
                        CompanyVote companyVoteAdd = new CompanyVote
                        {
                            CompanyID = companyVote.CompanyID,
                            UserID = companyVote.UserID,
                            VotedDate = DateTime.Now,
                        };

                        _context.CompanyVotes.Add(companyVoteAdd);
                        _context.SaveChanges();

                        User user = _context.Users.Where(u => u.UserID == companyVote.UserID).FirstOrDefault();
                        string companyName = _context.Company.Where(c => c.CompanyID == companyVote.CompanyID).Select(c => c.CompanyName).FirstOrDefault();
                        SendEmailForVoting(user, companyName);

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

        private void SendEmailForVoting(User user, string companyName)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"];
            emailProperties.EmailBCC = "support@upvotes.co; puneethm@hotmail.com";
            emailProperties.EmailSubject = user.FirstName + " " + user.LastName + " voted for " + companyName;
            emailProperties.EmailBody = GetVotingEmailContent(user, companyName).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private void SendEmailForUserVerification(int userID, string companyName, int companyID, string companyOTP, string workEmail)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = workEmail;
            emailProperties.EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com";
            emailProperties.EmailSubject = "Company profile verification at upvotes.co";
            emailProperties.EmailBody = GetUserVerificationEmailContent(userID, companyName, companyID, companyOTP).ToString();
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetUserVerificationEmailContent(int userID, string companyName, int companyID, string companyOTP)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "UserCompanyList/CompanyVerificationByUser?UID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(userID.ToString())) + "&CID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(companyOTP)) + "&KID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(companyID.ToString()));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p>");
            sb.Append("<p>Click the below link to verify your company profile at upvotes.co</p>");
            sb.Append("<p><em><strong><a href = '" + url + "' target = '_blank' rel = 'noopener'>" + url + "</a> &nbsp;</strong></em></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetVotingEmailContent(User user, string companyName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Hello,</p><p> We got a vote for&nbsp;<em><strong><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "profile/" + companyName.Replace(" ", "-") + "' target = '_blank' rel = 'noopener'>" + companyName + "</a> &nbsp;</strong></em> from &nbsp;<em><strong>"
                + user.FirstName + " " + user.LastName + ".</strong></em></p>");
            sb.Append("<p> Click <a href = '" + user.ProfileURL + "' target = '_blank' rel = 'noopener' > here </a> to know more about user.</p>");
            sb.Append("<p><a href = '" + user.ProfileURL + "' ><img src = '" + user.ProfilePictureURL + "' alt = '" + user.FirstName + " " + user.LastName + "' width = '80' height = '80' /></a></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private void SendEmailForQuotation(QuotationRequest request, QuotationResponse response)
        {
            Email emailProp = new Email();
            emailProp.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProp.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProp.EmailTo = request.EmailId;
            emailProp.EmailBCC = "support@upvotes.co; upvotes7@gmail.com; puneethm@hotmail.com";
            emailProp.EmailSubject = "Your Mobile App Development Cost Estimate";
            emailProp.EmailBody = GetQuotationEmailContent(request, response).ToString();
            EmailHelper.SendEmail(emailProp);
        }

        private string GetQuotationEmailContent(QuotationRequest request, QuotationResponse response)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("<p>Dear " + request.Name + ",</p><p> Thanks for trying out the Mobile App Cost Estimator. You can always find the details of your estimate by table below.</p>");
            sb.Append("<table style = 'font-family: arial, sans-serif;    border-collapse: collapse; width: 100%;'><tr><th style = 'background-color: #dddddd;'>Your Selection</th>");
            sb.Append("<th style = 'background-color: #dddddd;'> Zone 1 Amount </th>");
            sb.Append("<th style = 'background-color: #dddddd;'> Zone 2 Amount </th>");
            sb.Append("<th style = 'background-color: #dddddd;'> Zone 3 Amount </th></tr>");

            foreach (var item in response.QuotationData)
            {
                if (item.SubCategory != "Features")
                {
                    sb.Append("<tr><td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + item.Question + "<br/>" + item.Answer + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone1).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone1).ToString("#,##0") + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone2).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone2).ToString("#,##0") + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone3).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone3).ToString("#,##0") + "</td></tr>");
                }
                else
                {
                    sb.Append("<tr><td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + item.Ctypes + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone1).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone1).ToString("#,##0") + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone2).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone2).ToString("#,##0") + "</td>");
                    sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'>" + "$" + Convert.ToInt32(item.MinPriceZone3).ToString("#,##0") + " - " + "$" + Convert.ToInt32(item.MaxPriceZone3).ToString("#,##0") + "</td></tr>");
                }
            }

            sb.Append("<tr><td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'><strong>Total Approximate Cost</strong></td>");
            sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'><strong>" + "$" + Convert.ToInt32(response.TotalMinZone1).ToString("#,##0") + " - " + "$" + Convert.ToInt32(response.TotalMaxZone1).ToString("#,##0") + "</strong></td>");
            sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'><strong>" + "$" + Convert.ToInt32(response.TotalMinZone2).ToString("#,##0") + " - " + "$" + Convert.ToInt32(response.TotalMaxZone2).ToString("#,##0") + "</strong></td>");
            sb.Append("<td style = 'border: 1px solid #dddddd; text-align: left;padding: 8px;'><strong>" + "$" + Convert.ToInt32(response.TotalMinZone3).ToString("#,##0") + " - " + "$" + Convert.ToInt32(response.TotalMaxZone3).ToString("#,##0") + "</strong></td>");
            sb.Append("</tr></table><br/><p>If you decide to go ahead and build your app, we would love the opportunity to talk about how we can help.</p>");


            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private void SendCompanyApprovedEmail(string companyName, string workEmail, User newUserObj)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = workEmail;
            emailProperties.EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com";
            emailProperties.EmailSubject = companyName + " is approved at upvotes.co";
            emailProperties.EmailBody = GetAdminApprovedEmailContent(companyName, newUserObj);
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAdminApprovedEmailContent(string companyName, User newUserObj)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p> Your company profile " + companyName + " has been approved from admin at upvotes.co. Click on the below link to verify the contents.</p>");
            sb.Append("<p><em><strong><a href='" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "/profile/" + companyName.Replace(" ", "-").Trim().ToLower() + "' target='_blank' rel='noopener'>" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "/profile/" + companyName.Replace(" ", "-").Trim().ToLower() + "</a></strong></em><p>");
            if (newUserObj != null)
            {
                sb.Append("<p>Please use below credentials to login to the upvotes portal.</p>");
                sb.Append("<p> User Name:-" + newUserObj.UserName + "</p>");
                sb.Append("<p> Password:-" + newUserObj.UserPassword + "</p>");
            }

            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private void SendCompanyRejectedEmail(string companyName, string workEmail, string reason)
        {
            Email emailProperties = new Email();
            emailProperties.EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"];
            emailProperties.DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"];
            emailProperties.EmailTo = workEmail;
            emailProperties.EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com";
            emailProperties.EmailSubject = companyName + " is rejected at upvotes.co";
            emailProperties.EmailBody = GetAdminRejectedEmailContent(companyName, reason);
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAdminRejectedEmailContent(string companyName, string reason)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p> Your company profile " + companyName + " has been rejected from admin at upvotes.co due to the below reason </p>");
            sb.Append("<p><strong>" + reason + "</strong></p>");
            sb.Append("<p>Please correct to get it listed at upvotes.co</p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        public string ThanksNoteForReview(CompanyReviewThankNoteEntity companyReviewThanksNoteEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {

                    bool isThanksNoted = _context.CompanyReviewThankNotes.Where(e => e.CompanyReview.CompanyID == companyReviewThanksNoteEntity.CompanyID && e.UserID == companyReviewThanksNoteEntity.UserID && e.CompanyReviewID == companyReviewThanksNoteEntity.CompanyReviewID).Count() > 0 ? true : false;

                    if (isThanksNoted)
                    {
                        return "Duplicate thanks note.";
                    }
                    else
                    {
                        CompanyReviewThankNote companyReviewThankNoteAdd = new CompanyReviewThankNote
                        {
                            CompanyReviewID = companyReviewThanksNoteEntity.CompanyReviewID,
                            UserID = companyReviewThanksNoteEntity.UserID,
                            ThankNoteDate = DateTime.Now,
                        };

                        _context.CompanyReviewThankNotes.Add(companyReviewThankNoteAdd);
                        _context.SaveChanges();

                        return "Your thanks note is recorded for this review.";
                    }
                }

            }
            catch (Exception ex)
            {
                return "Something error occured while providing thanks note. Please contact support.";
            }
        }

        public List<string> GetDataForAutoComplete(int type, int focusAreaID, string searchTerm)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<string> myAutoCompleteList = new List<string>();

                    if (type == 1)//CompanyName
                    {
                        using (_context = new UpVotesEntities())
                        {
                            myAutoCompleteList = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetCompanyNames " + type + "," + focusAreaID + "," + searchTerm).Cast<string>().ToList();

                            return myAutoCompleteList;
                        }
                    }
                    else if (type == 2)//Location
                    {
                        using (_context = new UpVotesEntities())
                        {
                            myAutoCompleteList = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetCompanyNames " + type + "," + focusAreaID + "," + searchTerm).Cast<string>().ToList();

                            return myAutoCompleteList;
                        }
                    }

                    return myAutoCompleteList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CompanyDetail GetUserReviews(string companyName)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            CompanyEntity company = new CompanyEntity();
            company.CompanyName = companyName.Trim();
            company.CompanyReviews = GetCompanyReviews(company.CompanyName).ToList();
            if (company.CompanyReviews.Count() > 0)
            {
                foreach (CompanyReviewsEntity companyReviewsEntity in company.CompanyReviews)
                {
                    companyReviewsEntity.NoOfThankNotes = GetCompanyReviewThankNotes(company.CompanyID, companyReviewsEntity.CompanyReviewID).Count();
                }
            }

            companyDetail.CompanyList.Add(company);

            return companyDetail;
        }

        public CompanyDetail GetUserCompanies(int userID, string companyName)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<Company> companyListDb = new List<Company>();

                    if (companyName != "0")
                    {
                        companyListDb = _context.Company.Where(a => a.CompanyName.Trim().ToUpper() == companyName.Trim().ToUpper()).ToList();

                        foreach (Company companyDb in companyListDb)
                        {
                            CompanyEntity companyEntity = new CompanyEntity
                            {
                                CompanyID = companyDb.CompanyID,
                                CompanyName = companyDb.CompanyName,
                                LogoName = companyDb.LogoName,
                                TagLine = companyDb.TagLine,
                                FoundedYear = companyDb.FoundedYear,
                                TotalEmployees = Convert.ToString(companyDb.MinEmployeeCount + "-" + companyDb.MaxEmployeeCount),
                                AveragHourlyRate = Convert.ToString(companyDb.MinHourleyRate + "-" + companyDb.MaxHourleyRate),
                                URL = companyDb.URL,
                                LinkedInProfileURL = companyDb.LinkedInProfileURL,
                                TwitterProfileURL = companyDb.TwitterProfileURL,
                                FacebookProfileURL = companyDb.FacebookProfileURL,
                                GooglePlusProfileURL = companyDb.GooglePlusProfileURL,
                                Summary = companyDb.Summary,
                                KeyClients = companyDb.KeyClients,
                                WorkEmail = companyDb.WorkEmail,
                                IsUserVerified = companyDb.IsUserApproved == true ? "Yes" : "No",
                                IsAdminApproved = companyDb.IsAdminApproved == true ? "Yes" : "No",
                                Remarks = companyDb.Remarks,

                                CompanyBranches = GetCompanyBranches(companyDb.CompanyID).ToList(),
                                CompanyFocus = GetCompanyFocus(companyDb.CompanyID).ToList(),
                                CompanySubFocus = GetCompanySubFocus(companyDb.CompanyID).ToList(),
                                IndustialCompanyFocus = GetIndustrialFocus(companyDb.CompanyID).ToList(),
                                CompanyClientFocus = GetClientFocus(companyDb.CompanyID).ToList()
                            };

                            companyDetail.CompanyList.Add(companyEntity);
                        }
                    }
                    else
                    {
                        bool isAdminUser = _context.Users.Where(a => a.UserID == userID && a.UserType == 4).Count() > 0 ? true : false;
                        if (isAdminUser)
                        {
                            companyListDb = (from a in _context.Company join b in _context.CompanyPendingForApproval on a.CompanyID equals b.CompanyID where a.IsUserApproved == true && a.IsAdminApproved == false select a).Distinct().ToList();
                        }
                        else
                        {
                            companyListDb = _context.Database.SqlQuery(typeof(Company), "SELECT * FROM dbo.Company where CreatedBy = " + userID + " OR WorkEmail IN (SELECT UserName from Users where UserID= " + userID + ")").Cast<Company>().ToList();
                        }

                        foreach (Company companyDb in companyListDb)
                        {
                            CompanyEntity companyEntity = new CompanyEntity
                            {
                                CompanyID = companyDb.CompanyID,
                                CompanyName = companyDb.CompanyName,
                                FoundedYear = companyDb.FoundedYear,
                                URL = companyDb.URL,
                                IsUserVerified = companyDb.IsUserApproved == true ? "Yes" : "No",
                                UserVerifiedDate = companyDb.UserApprovedDate == null || companyDb.UserApprovedDate == DateTime.MinValue ? string.Empty : Convert.ToDateTime(companyDb.UserApprovedDate).ToString("dd-MMM-yyyy"),
                                IsAdminApproved = companyDb.IsAdminApproved == true ? "Yes" : "No",
                                AdminApprovedDate = companyDb.AdminApprovedDate == null || companyDb.AdminApprovedDate == DateTime.MinValue ? string.Empty : Convert.ToDateTime(companyDb.AdminApprovedDate).ToString("dd-MMM-yyyy"),
                                Remarks = companyDb.Remarks
                            };
                            companyDetail.CompanyList.Add(companyEntity);
                        }
                    }

                    companyDetail.FocusAreaList = new FocusAreaService().GetFocusAreaList();

                    return companyDetail;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public QuotationResponse GetQuotationData(QuotationRequest request)
        {
            QuotationResponse QuotationResponseobj = new QuotationResponse();
            QuotationInfo Quotationdetail = new QuotationInfo();
            QuotationResponseobj.QuotationData = new List<QuotationInfo>();

            try
            {
                using (_context = new UpVotesEntities())
                {
                    var quoteid = _context.Sp_InsUserQuotation(request.platform.ToString(), request.Theme.ToString(), request.LoginSecurity.ToString(), request.Profile.ToString(), request.Security.ToString(), request.ReviewRate.ToString(), request.Service.ToString(), request.Database.ToString(), request.featuresstring.ToString(), request.EmailId, request.Name, request.CompanyName);
                    var response = _context.Sp_GetQuoteForMobileApp(request.platform.ToString(), request.Theme.ToString(), request.LoginSecurity.ToString(), request.Profile.ToString(), request.Security.ToString(), request.ReviewRate.ToString(), request.Service.ToString(), request.Database.ToString(), request.featuresstring.ToString()).ToList();
                    if (response != null)
                    {
                        response.ToList().ForEach(q => QuotationResponseobj.QuotationData.Add(new QuotationInfo
                        {
                            QuotationRateCardID = q.QuotationRateCardID,
                            MainCategory = q.MainCategory,
                            SubCategory = q.SubCategory,
                            Ctypes = q.Ctypes,
                            Classname = q.Classname,
                            MinPriceZone1 = q.MinPriceZone1,
                            MaxPriceZone1 = q.MaxPriceZone1,
                            MinPriceZone2 = q.MinPriceZone2,
                            MaxPriceZone2 = q.MaxPriceZone2,
                            MinPriceZone3 = q.MinPriceZone3,
                            MaxPriceZone3 = q.MaxPriceZone3,
                            Question = q.Question,
                            Answer = q.Answer
                        }
                        ));
                        QuotationResponseobj.TotalMinZone1 = QuotationResponseobj.QuotationData.Sum(i => i.MinPriceZone1);
                        QuotationResponseobj.TotalMinZone2 = QuotationResponseobj.QuotationData.Sum(i => i.MinPriceZone2);
                        QuotationResponseobj.TotalMinZone3 = QuotationResponseobj.QuotationData.Sum(i => i.MinPriceZone3);
                        QuotationResponseobj.TotalMaxZone1 = QuotationResponseobj.QuotationData.Sum(i => i.MaxPriceZone1);
                        QuotationResponseobj.TotalMaxZone2 = QuotationResponseobj.QuotationData.Sum(i => i.MaxPriceZone2);
                        QuotationResponseobj.TotalMaxZone3 = QuotationResponseobj.QuotationData.Sum(i => i.MaxPriceZone3);

                        SendEmailForQuotation(request, QuotationResponseobj);
                    }
                }
                return QuotationResponseobj;
            }
            catch (Exception ex)
            {
                return QuotationResponseobj;
            }

        }

        public List<CountryEntity> GetCountry()
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<CountryEntity> countryList = (from a in _context.Countries
                                                       where a.IsActive == true
                                                       select new CountryEntity()
                                                       {
                                                           CountryID = a.CountryID,
                                                           CountryName = a.CountryName
                                                       }).ToList();

                    return countryList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<StateEntity> GetStates(int countryID)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<StateEntity> statesList = (from a in _context.States
                                                    where a.IsActive == true && a.CountryID == countryID
                                                    select new StateEntity()
                                                    {
                                                        StateID = a.StateID,
                                                        StateName = a.StateName
                                                    }).ToList();

                    return statesList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CompanyVerificationByUser(int uID, string cID, int compID)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Company userVerifiedCompany = _context.Company.Where(a => a.CompanyID == compID && a.CreatedBy == uID && a.CompanyOTP.Trim().ToUpper() == cID.Trim().ToUpper()).FirstOrDefault();
                    if (userVerifiedCompany != null)
                    {
                        userVerifiedCompany.IsUserApproved = true;
                        userVerifiedCompany.UserApprovedDate = DateTime.Now;
                        _context.SaveChanges();
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

        public bool UpdateRejectionComments(CompanyRejectComments companyRejectComments)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Company companyObj = _context.Company.Where(a => a.CompanyID == companyRejectComments.CompanyID).FirstOrDefault();
                    if (companyObj != null)
                    {
                        companyObj.Remarks += "Rejected By :- " + companyRejectComments.RejectedBy + " on " + DateTime.Now.ToLongDateString() + " Commets :-" + companyRejectComments.RejectComments + "\n";
                        companyObj.IsAdminApproved = false;
                        companyObj.AdminApprovedDate = null;
                        _context.SaveChanges();
                        SendCompanyRejectedEmail(companyObj.CompanyName, companyObj.WorkEmail, companyRejectComments.RejectComments);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
