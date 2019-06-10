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
    public class CompanyService : ICompanyService
    {
        private static Logger Log => Logger.Instance();

        private UpVotesEntities _context = null;

        public int SaveCompany(CompanyEntity companyEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Company companyObj = null; int companyId = 0;
                    bool isAdd = false; string companyOtp = string.Empty;
                    Sp_CheckForCompanyAndAdminUser_Result checkForCompanyAndAdminUserObj = _context.Sp_CheckForCompanyAndAdminUser(companyEntity.CompanyName, companyEntity.LoggedInUser).FirstOrDefault();
                    if (checkForCompanyAndAdminUserObj != null)
                    {
                        companyEntity.IsAdminUser = Convert.ToBoolean(checkForCompanyAndAdminUserObj.IsAdmin);

                        if (companyEntity.CompanyID == 0)
                        {
                            if (Convert.ToBoolean(checkForCompanyAndAdminUserObj.IsCompanyExists))
                            {
                                return 0;
                            }

                            isAdd = true;
                            companyId = Convert.ToInt32(checkForCompanyAndAdminUserObj.PreviousCompanyID);
                            companyId = companyId + 1;
                            companyEntity.CompanyID = companyId;

                            companyObj = new Company();
                            CompanyInitialization(companyEntity, companyObj, true);
                            companyObj.CreatedBy = companyEntity.CreatedBy;
                            companyObj.CreatedDate = DateTime.Now;
                            companyObj.CompanyOTP = new Random().Next(100000, 999999).ToString("D6");
                            companyObj.IsActive = true;
                            _context.Company.Add(companyObj);
                        }
                        else
                        {
                            isAdd = false;
                            companyObj = _context.Company.FirstOrDefault(c => c.CompanyID == companyEntity.CompanyID);
                            CompanyInitialization(companyEntity, companyObj, false);
                            companyObj.ModifiedBy = companyEntity.LoggedInUser;
                            companyObj.ModifiedDate = DateTime.Now;
                            companyId = companyEntity.CompanyID;

                            if (!companyEntity.IsAdminUser && companyObj.IsAdminApproved == true &&
                                companyObj.IsUserApproved == true)
                            {
                                _context.SP_CopyCompany(companyEntity.CompanyID);
                            }
                        }
                    }

                    _context.SaveChanges();

                    if (companyEntity.CompanyFocus.Any())
                    {
                        AddCompanyFocusAreas(companyEntity);
                    }

                    if (companyEntity.CompanyBranches.Any())
                    {
                        AddCompanyBranches(companyEntity);
                    }

                    if (isAdd)
                    {
                        if (!companyEntity.IsAdminUser)
                        {
                            SendEmailForUserVerification(companyEntity.LoggedInUser, companyObj.CompanyName, companyId, companyObj.CompanyOTP, companyObj.WorkEmail, false);
                        }
                    }

                    if (!companyEntity.IsAdminUser)
                    {
                        string insertQuery = "IF NOT EXISTS (SELECT * FROM dbo.CompanyPendingForApproval WHERE CompanyID = " + companyEntity.CompanyID + ") BEGIN INSERT INTO CompanyPendingForApproval (CompanyID) VALUES (" + companyEntity.CompanyID + ") END";
                        _context.Database.ExecuteSqlCommand(insertQuery, companyId);
                        //companyObj.IsAdminApproved = false;
                    }
                    else
                    {
                        if (!isAdd && companyObj.IsUserApproved)
                        {
                            string deleteQuery = "DELETE FROM dbo.CompanyPendingForApproval WHERE CompanyID =" + companyEntity.CompanyID;
                            _context.Database.ExecuteSqlCommand(deleteQuery, companyId);
                            if (!companyObj.IsAdminApproved)
                            {
                                User newUserObj = AddUserByWorkEmailID(companyObj, _context);
                                SendCompanyApprovedEmail(companyObj.CompanyName, companyObj.WorkEmail, newUserObj);
                                companyObj.CreatedBy = newUserObj.UserID;
                            }
                            companyObj.IsAdminApproved = true;
                            companyObj.AdminApprovedDate = DateTime.Now;

                            _context.Sp_DeleteCompanyHistory(companyEntity.CompanyID);
                        }
                        //else
                        //{
                        //    companyObj.IsAdminApproved = true;
                        //    companyObj.AdminApprovedDate = DateTime.Now;
                        //}
                    }

                    _context.SaveChanges();

                    return companyId;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveUpdateCompanyPortFolio(CompanyPortFolioEntity companyEntity)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    int? portfolioID = _context.Sp_InsUpdCompanyPortFolio(companyEntity.CompanyPortFolioID, companyEntity.CompanyID, companyEntity.Title, companyEntity.Description, companyEntity.CreatedBy, companyEntity.ImageURL).FirstOrDefault();
                    return Convert.ToInt32(portfolioID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddCompanyBranches(CompanyEntity companyEntity)
        {
            CompanyBranch companyBranchObj = null;
            string deleteBranchQuery = "DELETE FROM dbo.CompanyBranch WHERE CompanyID = " + companyEntity.CompanyID;
            _context.Database.ExecuteSqlCommand(deleteBranchQuery);

            int companyBranchID = _context.CompanyBranches.OrderByDescending(a => a.BranchID).Select(a => a.BranchID).FirstOrDefault();
            int j = 0;
            foreach (CompanyBranchEntity companyBranchEntityObj in companyEntity.CompanyBranches)
            {
                j = j + 1;
                companyBranchObj = new CompanyBranch
                {
                    BranchID = companyBranchID + j,
                    BranchName = companyBranchEntityObj.BranchName,
                    CompanyID = companyEntity.CompanyID,
                    CountryID = companyBranchEntityObj.CountryID,
                    StateID = companyBranchEntityObj.StateID,
                    Address = companyBranchEntityObj.Address,
                    City = companyBranchEntityObj.City,
                    PostalCode = companyBranchEntityObj.PostalCode,
                    PhoneNumber = companyBranchEntityObj.PhoneNumber,
                    Email = companyBranchEntityObj.Email,
                    IsHeadQuarters = companyBranchEntityObj.IsHeadQuarters,
                    IsActive = true,
                    CreatedBy = companyEntity.LoggedInUser,
                    CreatedDate = DateTime.Now
                };
                _context.CompanyBranches.Add(companyBranchObj);
            }

            _context.SaveChanges();
        }

        private void AddCompanyFocusAreas(CompanyEntity companyEntity)
        {
            CompanyFocus companyFocusObj = null;
            string deleteFocusQuery = "DELETE A FROM dbo.CompanySubFocus A JOIN dbo.CompanyFocus B ON A.CompanyFocusID = B.CompanyFocusID WHERE B.CompanyID =" + companyEntity.CompanyID + "  DELETE FROM dbo.CompanyFocus WHERE CompanyID=" + companyEntity.CompanyID;
            _context.Database.ExecuteSqlCommand(deleteFocusQuery);

            int companyFocusID = _context.CompanyFocus.OrderByDescending(a => a.CompanyFocusID).Select(a => a.CompanyFocusID).FirstOrDefault();
            int i = 0;
            foreach (CompanyFocusEntity companyFocusEntityObj in companyEntity.CompanyFocus)
            {
                i = i + 1;
                companyFocusObj = new CompanyFocus
                {
                    CompanyID = companyEntity.CompanyID,
                    CompanyFocusID = companyFocusID + i,
                    FocusAreaID = companyFocusEntityObj.FocusAreaID,
                    FocusAreaPercentage = companyFocusEntityObj.FocusAreaPercentage,
                    IsActive = true,
                    CreatedBy = companyEntity.LoggedInUser,
                    CreatedDate = DateTime.Now
                };
                _context.CompanyFocus.Add(companyFocusObj);
                //_context.SaveChanges();

                CompanySubFocus dbCompanySubFocusObj = null;
                foreach (CompanySubFocusEntity companySubFocusEntityObj in companyFocusEntityObj.CompanySubFocus)
                {
                    dbCompanySubFocusObj = new CompanySubFocus
                    {
                        CompanyFocusID = companyFocusID + i,
                        SubFocusAreaID = companySubFocusEntityObj.SubFocusAreaID,
                        SubFocusAreaPercentage = companySubFocusEntityObj.SubFocusAreaPercentage,
                        IsActive = true,
                        CreatedBy = companyEntity.LoggedInUser,
                        CreatedDate = DateTime.Now
                    };
                    _context.CompanySubFocus.Add(dbCompanySubFocusObj);
                    // _context.SaveChanges();
                }
            }

            _context.SaveChanges();
        }

        private User AddUserByWorkEmailID(Company companyObj, UpVotesEntities _upvotesContext)
        {
            User dbUser = _upvotesContext.Users.FirstOrDefault(a => a.UserName.Trim().ToUpper() == companyObj.WorkEmail.Trim().ToUpper());

            if (dbUser == null)
            {
                dbUser = _upvotesContext.Users.Where(a => a.UserID == companyObj.CreatedBy).FirstOrDefault();
                if (dbUser != null)
                {
                    User dbNewUser = new User()
                    {
                        UserName = companyObj.WorkEmail.Trim(),
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

            dbUser = _upvotesContext.Users.FirstOrDefault(a => a.UserName.Trim().ToUpper() == companyObj.WorkEmail.Trim().ToUpper());

            companyObj.CreatedBy = dbUser.UserID;

            return dbUser;
        }

        private void CompanyInitialization(CompanyEntity companyEntity, Company companyObj, bool isAdd)
        {
            companyObj.CompanyID = companyEntity.CompanyID;
            companyObj.CompanyName = companyEntity.CompanyName;
            if (!string.IsNullOrEmpty(companyEntity.LogoName))
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
            if (companyEntity.IsAdminUser && companyEntity.CreatedBy == companyEntity.LoggedInUser)
            {
                companyObj.CompanyDomain = companyEntity.CompanyDomain;
            }
            else
            {
                companyObj.WorkEmail = companyEntity.WorkEmail;
            }
        }

        public string DeleteCompany(int companyId)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    if (_context.Sp_DeleteCompany(companyId) > 0)
                    {
                        return "Company Profile is deleted";
                    }                    
                }
            }
            catch (Exception e)
            {                
                throw e;
            }

            return "Error in deleting the company";
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
            OverviewNewsService newsObj = new OverviewNewsService();
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
                            company.CompanyFocus = GetCompanyListFocus(company.CompanyID, Convert.ToInt32(focusAreaID), "0").ToList();
                            //company.CompanyFocus = GetCompanyFocus(company.CompanyID).ToList();
                            company.IndustialCompanyFocus = GetIndustrialFocus(company.CompanyID).ToList();
                            company.CompanyClientFocus = GetClientFocus(company.CompanyID).ToList();
                            company.SubfocusNames = GetDistinctSubFocusNames(company.CompanyID).ToList();
                            company.CompanySubFocus = GetCompanySubFocus(company.CompanyID).ToList();
                            company.CompanyBranches = GetCompanyBranches(company.CompanyID).ToList();
                            company.CompanyPortFolio = GetCompanyPortFolio(company.CompanyID, 5).ToList();
                            company.CompanyTeamMembers = new TeamMemberHelper().GetTeamMembers(0, company.CompanyName, 5);// To get top 5 company employees
                            company.CompanyReviews = GetCompanyReviews(company.CompanyName, 5).ToList();
                            company.OverviewNewsData = newsObj.GetCompanySoftwareNewsByID(1, company.CompanyID);
                            //if (company.CompanyReviews.Count() > 0)
                            //{
                            //    foreach (CompanyReviewsEntity companyReviewsEntity in company.CompanyReviews)
                            //    {
                            //        companyReviewsEntity.NoOfThankNotes = GetCompanyReviewThankNotes(company.CompanyID, companyReviewsEntity.CompanyReviewID).Count();
                            //    }
                            //}
                        }
                        else
                        {
                            company.CompanyFocus = GetCompanyListFocus(company.CompanyID, Convert.ToInt32(focusAreaID), subFocusArea).ToList();
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

        private IEnumerable<CompanyPortFolioEntity> GetCompanyPortFolio(int companyID, int rows)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyPortFolio_Result> companyPortFolio = _context.Database.SqlQuery(typeof(Sp_GetCompanyPortFolio_Result), "EXEC Sp_GetCompanyPortFolio " + companyID + "," + rows + "").Cast<Sp_GetCompanyPortFolio_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyPortFolio_Result, CompanyPortFolioEntity>(); });
                IEnumerable<CompanyPortFolioEntity> companyPortFolioEntity = Mapper.Map<IEnumerable<Sp_GetCompanyPortFolio_Result>, IEnumerable<CompanyPortFolioEntity>>(companyPortFolio);
                return companyPortFolioEntity;
            }
        }

        private IEnumerable<CompanyPortFolioEntity> GetCompanyPortFolioByName(string companyName, int rows)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyPortFolioByName_Result> companyPortFolio = _context.Database.SqlQuery(typeof(Sp_GetCompanyPortFolioByName_Result), "EXEC Sp_GetCompanyPortFolioByName '" + companyName + "'," + rows).Cast<Sp_GetCompanyPortFolioByName_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyPortFolioByName_Result, CompanyPortFolioEntity>(); });
                IEnumerable<CompanyPortFolioEntity> companyPortFolioEntity = Mapper.Map<IEnumerable<Sp_GetCompanyPortFolioByName_Result>, IEnumerable<CompanyPortFolioEntity>>(companyPortFolio);
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

        private IEnumerable<CompanyFocusEntity> GetCompanyListFocus(int companyID, int focusAreaID, string subfocusArea)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyListFocus_Result> companyFocus = _context.Database.SqlQuery(typeof(Sp_GetCompanyListFocus_Result), "EXEC Sp_GetCompanyListFocus " + companyID + "," + focusAreaID + ",'" + subfocusArea + "'").Cast<Sp_GetCompanyListFocus_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyListFocus_Result, CompanyFocusEntity>(); });
                IEnumerable<CompanyFocusEntity> companyFocusEntity = Mapper.Map<IEnumerable<Sp_GetCompanyListFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyFocus);
                return companyFocusEntity;
            }
        }

        private IEnumerable<CategoryMetaTags> CategoryMetaTags(string FocusAreaName, string SubFocusAreaName)
        {
            using (_context = new UpVotesEntities())
            {
                bool IsService = true;
                IEnumerable<Sp_CategoryMetaTags_Result> metaTag = _context.Database.SqlQuery(typeof(Sp_CategoryMetaTags_Result), "EXEC Sp_CategoryMetaTags '" + FocusAreaName + "','" + SubFocusAreaName + "'," + IsService).Cast<Sp_CategoryMetaTags_Result>().AsEnumerable();
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

        private IEnumerable<CompanyReviewsEntity> GetCompanyReviews(string companyName, int noOfRows)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyReviews_Result> companyReviews = _context.Database.SqlQuery(typeof(Sp_GetCompanyReviews_Result), "EXEC Sp_GetCompanyReviews " + "'" + companyName + "'," + noOfRows).Cast<Sp_GetCompanyReviews_Result>().AsEnumerable();
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
                    decimal? isVoted = _context.Sp_CompanyVote(companyVote.UserID, companyVote.CompanyID).FirstOrDefault();
                    if (isVoted == -1)
                    {
                        return "You already voted for this company.";
                    }
                    else
                    {
                        Sp_CompanyVoteUserInformation_Result user = _context.Sp_CompanyVoteUserInformation(companyVote.UserID, companyVote.CompanyID).FirstOrDefault();
                        Thread thread = new Thread(() => SendEmailForVoting(user, user.CompanyName));
                        thread.Start();

                        //User user = _context.Users.Where(u => u.UserID == companyVote.UserID).FirstOrDefault();
                        //string companyName = _context.Company.Where(c => c.CompanyID == companyVote.CompanyID).Select(c => c.CompanyName).FirstOrDefault();
                        //SendEmailForVoting(user, user.CompanyName);

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

        private void SendEmailForVoting(Sp_CompanyVoteUserInformation_Result user, string companyName)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = System.Configuration.ConfigurationManager.AppSettings["EmailTo"],
                EmailBCC = "support@upvotes.co; puneethm@hotmail.com",
                EmailSubject = user.FirstName + " " + user.LastName + " voted for " + companyName,
                EmailBody = GetVotingEmailContent(user, companyName).ToString()
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private void SendEmailForUserVerification(int userID, string companyName, int companyID, string companyOTP, string workEmail, bool IsClaim)
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
                emailProperties.EmailSubject = "Company profile verification at upvotes.co";
                emailProperties.EmailBody = GetUserVerificationEmailContent(userID, companyName, companyID, companyOTP).ToString();
            }
            else
            {
                emailProperties.EmailSubject = "Company profile claimed at upvotes.co";
                emailProperties.EmailBody = GetUserClaimedEmailContent(userID, companyName, companyID, workEmail).ToString();
            }

            EmailHelper.SendEmail(emailProperties);
        }

        private string GetUserVerificationEmailContent(int userID, string companyName, int companyID, string companyOTP)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "UserCompanyList/CompanyVerificationByUser?UID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(userID.ToString())) + "&CID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(companyOTP)) + "&KID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(companyID.ToString()));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p>");
            sb.Append("<p>Click the below link to verify your company profile " + companyName + " at upvotes.co</p>");
            sb.Append("<p><em><strong><a href = '" + url + "' target = '_blank' rel = 'noopener'>" + url + "</a> &nbsp;</strong></em></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetUserClaimedEmailContent(int claimID, string companyName, int companyID, string workEmail)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "company/claimcompanyverification?CID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(claimID.ToString())) + "&WID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(workEmail)) + "&KID=" + System.Web.HttpUtility.UrlEncode(EncryptionAndDecryption.Encrypt(companyID.ToString()));

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p>");
            sb.Append("<p>Click the below link to verify your claimed company profile at upvotes.co</p>");
            sb.Append("<p><em><strong><a href = '" + url + "' target = '_blank' rel = 'noopener'>" + url + "</a> &nbsp;</strong></em></p>");
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetVotingEmailContent(Sp_CompanyVoteUserInformation_Result user, string companyName)
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
            Email emailProp = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = request.EmailId,
                EmailBCC = "support@upvotes.co; upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = "Your Mobile App Development Cost Estimate",
                EmailBody = GetQuotationEmailContent(request, response).ToString()
            };
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

            foreach (QuotationInfo item in response.QuotationData)
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
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = workEmail,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = companyName + " is updated at upvotes.co",
                EmailBody = GetAdminApprovedEmailContent(companyName, newUserObj)
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private void SendApproveRejectEmailForClaimedUser(string CompanyName, string Email, string password, bool IsAdminApproved, string RejectionComment, string Type)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = Email,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com"
            };
            if (IsAdminApproved)
            {
                emailProperties.EmailSubject = CompanyName + " is approved at upvotes.co";
            }
            else
            {
                emailProperties.EmailSubject = CompanyName + " is rejected at upvotes.co";
            }
            emailProperties.EmailBody = GetAdminApprovedRejectedEmailContentForClaimedUser(CompanyName, Email, password, IsAdminApproved, RejectionComment, Type);
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetAdminApprovedRejectedEmailContentForClaimedUser(string CompanyName, string Email, string password, bool IsAdminApproved, string RejectionComment, string Type)
        {
            string type = "profile/";
            if (Type == "software")
            {
                type = "software/";
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (IsAdminApproved)
            {
                sb.Append("<p>Hello,</p><p> Your claimed company profile " + CompanyName + " has been approved at upvotes.co. Click on the below link to verify the contents.</p>");
                sb.Append("<p><em><strong><a href='" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + type + CompanyName.Replace(" ", "-").Trim().ToLower() + "' target='_blank' rel='noopener'>" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + type + CompanyName.Replace(" ", "-").Trim().ToLower() + "</a></strong></em><p>");
                sb.Append("<p>Please use below credentials to login to the upvotes portal.</p>");
                sb.Append("<p> User Name:<br/>" + Email + "</p>");
                sb.Append("<p> Password:<br/>" + EncryptionAndDecryption.Decrypt(password) + "</p>");
            }
            else
            {
                sb.Append("<p>Hello,</p><p> Your claimed company profile " + CompanyName + " has been rejected at upvotes.co.</p>");
                sb.Append("<p>Below is the reason for rejection.</p>");
                sb.Append("<p>" + RejectionComment + "</p>");
            }
            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private string GetAdminApprovedEmailContent(string companyName, User newUserObj)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p> Your company profile " + companyName + " has been updated at upvotes.co. Click on the below link to verify the contents.</p>");
            sb.Append("<p><em><strong><a href='" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "profile/" + companyName.Replace(" ", "-").Trim().ToLower() + "' target='_blank' rel='noopener'>" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "profile/" + companyName.Replace(" ", "-").Trim().ToLower() + "</a></strong></em><p>");
            if (newUserObj != null)
            {
                sb.Append("<p>Please use below credentials to login to the upvotes portal.</p>");
                sb.Append("<p> User Name:<br/>" + newUserObj.UserName + "</p>");
                sb.Append("<p> Password:<br/>" + EncryptionAndDecryption.Decrypt(newUserObj.UserPassword) + "</p>");
            }

            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
        }

        private void SendCompanyRejectedEmail(string companyName, string workEmail, string reason)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = workEmail,
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = companyName + " is rejected at upvotes.co",
                EmailBody = GetAdminRejectedEmailContent(companyName, reason)
            };
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
                    decimal? isThankNote = _context.Sp_InsCompanyReviewForThanksNote(companyReviewThanksNoteEntity.UserID, companyReviewThanksNoteEntity.CompanyReviewID).FirstOrDefault();
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

        public CompanyDetail GetUserReviews(string companyName, int noOfRows)
        {
            CompanyDetail companyDetail = new CompanyDetail
            {
                CompanyList = new List<CompanyEntity>()
            };

            CompanyEntity company = new CompanyEntity
            {
                CompanyName = companyName.Trim()
            };
            company.CompanyReviews = GetCompanyReviews(company.CompanyName, noOfRows).ToList();

            companyDetail.CompanyList.Add(company);

            return companyDetail;
        }

        public CompanyDetail GetAllCompanyPortfolioByName(string companyName, int noOfRows)
        {
            CompanyDetail companyDetail = new CompanyDetail
            {
                CompanyList = new List<CompanyEntity>()
            };

            CompanyEntity company = new CompanyEntity
            {
                CompanyName = companyName.Trim()
            };
            company.CompanyPortFolio = GetCompanyPortFolioByName(company.CompanyName, noOfRows).ToList();

            companyDetail.CompanyList.Add(company);

            return companyDetail;
        }

        public List<CompanyPortFolioEntity> GetCompanyPortfolioByID(int CompanyID, int Rows)
        {
            List<CompanyPortFolioEntity> companyPortfolioObj = new List<CompanyPortFolioEntity>();
            companyPortfolioObj = GetCompanyPortFolio(CompanyID, Rows).ToList();
            return companyPortfolioObj;
        }

        public int DeleteCompanyPortfolio(int portfolioID)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    int? deleted = _context.Sp_DeletePortfolio(portfolioID).FirstOrDefault();
                    return Convert.ToInt32(deleted);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CompanyPortFolioEntity GetPortfolioInfoByID(int portfolioID)
        {
            CompanyPortFolioEntity PortfolioObj = new CompanyPortFolioEntity();
            try
            {
                using (_context = new UpVotesEntities())
                {
                    Sp_GetPortfolioInfoByID_Result response = _context.Sp_GetPortfolioInfoByID(portfolioID).FirstOrDefault();
                    if (response != null)
                    {
                        PortfolioObj.CompanyID = response.CompanyID;
                        PortfolioObj.CompanyPortFolioID = response.CompanyPortFolioID;
                        PortfolioObj.Description = response.Description;
                        PortfolioObj.ImageURL = response.ImageURL;
                        PortfolioObj.Title = response.Title;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return PortfolioObj;
        }

        public CompanySoftwareReviews GetReviewsForCompanyListingPage(CompanyFilterEntity companyFilter)
        {
            CompanySoftwareReviews companyReviewsObj = new CompanySoftwareReviews
            {
                ReviewsList = new List<CompanyReviewsEntity>(),
                CompanyNamesList = new List<string>()
            };


            using (_context = new UpVotesEntities())
            {
                List<Sp_GetCompanyReviewsForListingPage_Result> response = _context.Sp_GetCompanyReviewsForListingPage(companyFilter.CompanyName, companyFilter.FocusAreaID, 5, companyFilter.PageNo, companyFilter.PageSize).ToList();
                if (response != null)
                {
                    response.ToList().ForEach(q => companyReviewsObj.ReviewsList.Add(new CompanyReviewsEntity
                    {
                        CompanyReviewID = q.CompanyReviewID,
                        CompanyID = q.CompanyId,
                        CompanyName = q.CompanyName,
                        ReviewerFullName = q.ReviewerFullName,
                        ReviewerCompanyName = q.ReviewerCompanyName,
                        ReviewerDesignation = q.ReviewerDesignation,
                        ReviewerProjectName = q.ReviewerProjectName,
                        FocusAreaName = q.FocusAreaName,
                        FeedBack = q.FeedBack,
                        Rating = q.Rating,
                        ReviewDate = q.ReviewDate,
                        UserProfilePicture = q.UserProfilePicture,
                        NoOfReviews = q.NoOfReviews,
                        NoOfThankNotes = q.ThankYouCount
                    }
                ));
                    companyReviewsObj.CompanyNamesList = response.ToList().Select(i => i.CompanyName).Distinct().ToList();

                }
            }

            return companyReviewsObj;
        }

        public CompanyDetail GetUserCompanies(int userID, string companyName)
        {
            CompanyDetail companyDetail = new CompanyDetail { CompanyList = new List<CompanyEntity>() };

            bool isAdminUser = false;
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
                                Summary1 = companyDb.Summary1 ?? string.Empty,
                                Summary2 = companyDb.Summary2 ?? string.Empty,
                                Summary3 = companyDb.Summary3 ?? string.Empty,
                                KeyClients = companyDb.KeyClients,
                                WorkEmail = companyDb.WorkEmail,
                                IsUserVerified = companyDb.IsUserApproved == true ? "Yes" : "No",
                                IsAdminApproved = companyDb.IsAdminApproved == true ? "Yes" : "No",
                                Remarks = companyDb.Remarks,
                                CreatedBy = Convert.ToInt32(companyDb.CreatedBy),
                                CompanyDomain = companyDb.CompanyDomain,

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
                        isAdminUser = _context.Users.Any(a => a.UserID == userID && a.UserType == 4) ? true : false;

                        if (isAdminUser)
                        {
                            //companyListDb = (from a in _context.Company join b in _context.CompanyPendingForApproval on a.CompanyID equals b.CompanyID where a.IsUserApproved == true  select a).Distinct().ToList();
                            companyListDb = _context.Database.SqlQuery(typeof(Company), "SELECT A.* FROM dbo.Company A LEFT OUTER JOIN dbo.CompanyPendingForApproval B ON A.CompanyID = B.CompanyID WHERE A.IsUserApproved=1 OR A.CreatedBy = " + userID).Cast<Company>().ToList();
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
                                CreatedBy = Convert.ToInt32(companyDb.CreatedBy),
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

        public CompanyDetail GetClaimListingsForApproval(int userID)
        {
            CompanyDetail companyDetail = new CompanyDetail
            {
                ClaimList = new List<ClaimInfoDetail>()
            };
            bool isAdminUser = false;
            try
            {
                using (_context = new UpVotesEntities())
                {
                    isAdminUser = _context.Users.Where(a => a.UserID == userID && a.UserType == 4).Count() > 0 ? true : false;
                    if (isAdminUser)
                    {
                        List<Sp_GetClaimListingsForApproval_Result> response = _context.Sp_GetClaimListingsForApproval(userID).ToList();
                        if (response != null)
                        {
                            response.ToList().ForEach(q => companyDetail.ClaimList.Add(new ClaimInfoDetail
                            {
                                ClaimListingID = q.ClaimListingID,
                                CompanyID = q.CompanyID,
                                CompanyDomain = q.CompanyDomain,
                                CompanyName = q.CompanyName,
                                IsUserApproved = q.IsUserApproved,
                                URL = q.URL,
                                WorkEmail = q.WorkEmail,
                                ProfileURL = q.ProfileURL,
                                UpvotesURL = q.UpvotesURL,
                                UserApprovedDate = q.UserApprovedDate,
                                IsSoftware = q.IsSoftware
                            }
                        ));
                        }
                    }

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
                    System.Data.Entity.Core.Objects.ObjectResult<decimal?> quoteid = _context.Sp_InsUserQuotation(request.platform.ToString(), request.Theme.ToString(), request.LoginSecurity.ToString(), request.Profile.ToString(), request.Security.ToString(), request.ReviewRate.ToString(), request.Service.ToString(), request.Database.ToString(), request.featuresstring.ToString(), request.EmailId, request.Name, request.CompanyName);
                    List<Sp_GetQuoteForMobileApp_Result> response = _context.Sp_GetQuoteForMobileApp(request.platform.ToString(), request.Theme.ToString(), request.LoginSecurity.ToString(), request.Profile.ToString(), request.Security.ToString(), request.ReviewRate.ToString(), request.Service.ToString(), request.Database.ToString(), request.featuresstring.ToString()).ToList();
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
            catch (Exception)
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

                    //List<CountryEntity> countryList = (from a in _context.Countries
                    //                                   where a.IsActive == true
                    //                                   select new CountryEntity()
                    //                                   {
                    //                                       CountryID = a.CountryID,
                    //                                       CountryName = a.CountryName
                    //                                   }).ToList();
                    List<CountryEntity> countryList = new List<CountryEntity>();
                    List<Sp_GetCountry_Result> List = _context.Sp_GetCountry().ToList();
                    if (List != null)
                    {
                        List.ForEach(a => countryList.Add(new CountryEntity
                        {
                            CountryID = a.CountryID,
                            CountryName = a.CountryName
                        }));
                    }
                    return countryList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CategoryLinksEntity> GetServiceCategoryLinks(int focusAreaID)
        {
            List<CategoryLinksEntity> LinkObj = new List<CategoryLinksEntity>();
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<Sp_GetAllCategoriesLinks_Result> response = _context.Sp_GetAllCategoriesLinks(focusAreaID).ToList();

                    if (response != null)
                    {
                        response.ToList().ForEach(v => LinkObj.Add(new CategoryLinksEntity
                        {
                            Name = v.Name,
                            Category = v.Category,
                            CountOfListing = Convert.ToInt32(v.CountOfListing),
                            URL = v.URL,
                            cityName = v.cityName,
                            cityCategory = v.cityCategory,
                            cityCountOfListing = Convert.ToInt32(v.cityCountOfListing),
                            cityURL = v.cityURL,
                            countryName = v.countryName,
                            countryCategory = v.countryCategory,
                            countryCountOfListing = Convert.ToInt32(v.countryCountOfListing),
                            countryURL = v.countryURL,
                            stateName = v.stateName,
                            stateCategory = v.stateCategory,
                            stateCountOfListing = Convert.ToInt32(v.stateCountOfListing),
                            stateURL = v.stateURL,
                        }));
                    }
                    return LinkObj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CompanyEntity> GetTopVoteCompanies()
        {
            List<CompanyEntity> TopVotedCompaniesObj = new List<CompanyEntity>();
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<Sp_GetTopVotesCompany_Result> response = _context.Sp_GetTopVotesCompany().ToList();

                    if (response != null)
                    {
                        response.ToList().ForEach(v => TopVotedCompaniesObj.Add(new CompanyEntity
                        {
                            NoOfVotes = Convert.ToInt32(v.vote),
                            CompanyID = v.CompanyID,
                            CompanyName = v.CompanyName,
                            LogoName = v.LogoName
                        }));
                    }
                    return TopVotedCompaniesObj;
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
                    //List<StateEntity> statesList = (from a in _context.States
                    //                                where a.IsActive == true && a.CountryID == countryID
                    //                                select new StateEntity()
                    //                                {
                    //                                    StateID = a.StateID,
                    //                                    StateName = a.StateName
                    //                                }).ToList();
                    List<StateEntity> statesList = new List<StateEntity>();
                    List<Sp_GetStateByCountryID_Result> list = _context.Sp_GetStateByCountryID(countryID).ToList();
                    if (list != null)
                    {
                        list.ForEach(a => statesList.Add(new StateEntity
                        {
                            StateID = a.StateID,
                            StateName = a.StateName
                        }));
                    }

                    return statesList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SubFocusAreaEntity> GetSubFocusAreaByFocusID(int FocusID)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<SubFocusAreaEntity> subFocusList = new List<SubFocusAreaEntity>();
                    List<Sp_GetSubFocusAreaByFocusID_Result> list = _context.Sp_GetSubFocusAreaByFocusID(FocusID).ToList();
                    if (list != null)
                    {
                        list.ForEach(a => subFocusList.Add(new SubFocusAreaEntity
                        {
                            SubFocusAreaID = a.SubFocusAreaID,
                            SubFocusAreaName = a.SubFocusAreaName
                        }));
                    }

                    return subFocusList;
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
                        SendUserApprovedEmail(userVerifiedCompany.CompanyName);
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

        private void SendUserApprovedEmail(string companyName)
        {
            Email emailProperties = new Email
            {
                EmailFrom = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                DomainDisplayName = System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"],
                EmailTo = System.Configuration.ConfigurationManager.AppSettings["AdminEmail"],
                EmailBCC = "upvotes7@gmail.com; puneethm@hotmail.com",
                EmailSubject = companyName + " is approved by user.",
                EmailBody = GetUserApprovedEmailContent(companyName)
            };
            EmailHelper.SendEmail(emailProperties);
        }

        private string GetUserApprovedEmailContent(string companyName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<p>Hello,</p><p>Hello Admin, <br/> <b>" + companyName + "<b/> has been approved from User.</p>");

            EmailHelper.GetEmailSignature(sb);
            return sb.ToString();
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
                        companyObj.Remarks += "Rejected By : " + companyRejectComments.RejectedBy + " on " + DateTime.Now.ToLongDateString() + " Comments : " + companyRejectComments.RejectComments + "\n";
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

        public string InsertUpdateClaimListing(ClaimApproveRejectListingRequest request)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    int? ClaimId = _context.Sp_InsertUpdateClaimListing(request.ClaimListingID, request.companyID, request.userID, request.IsUserVerify, request.Email + request.Domain).FirstOrDefault();
                    if (request.ClaimListingID == 0)
                    {
                        Thread thread = new Thread(() => SendEmailForUserVerification(Convert.ToInt32(ClaimId), request.CompanyName, request.companyID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true));
                        thread.Start();
                        //SendEmailForUserVerification(Convert.ToInt32(ClaimId), request.CompanyName, request.companyID, new Random().Next(100000, 999999).ToString("D6"), request.Email + request.Domain, true);
                    }
                    return "OK";
                }

            }
            catch (Exception)
            {
                return "Error";
            }

        }

        public string AdminApproveRejectForClaiming(ClaimApproveRejectListingRequest request)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    string pwd = EncryptionAndDecryption.Encrypt(EncryptionAndDecryption.GenRandomAlphaNum(6));
                    int result = _context.Sp_AdminApproveRejectForClaiming(request.ClaimListingID, request.userID, request.companyID, request.IsAdminApproved, pwd);
                    if (result != -1)
                    {
                        Thread thread = new Thread(() => SendApproveRejectEmailForClaimedUser(request.CompanyName, request.Email, pwd, request.IsAdminApproved, request.RejectionComment, request.Type));
                        thread.Start();
                        //SendApproveRejectEmailForClaimedUser(request.CompanyName, request.Email, pwd, request.IsAdminApproved, request.RejectionComment, request.Type);
                        if (request.IsAdminApproved)
                        {
                            return "claimed";
                        }
                        else
                        {
                            return "Rejected";
                        }
                    }
                    else
                    {
                        return "error";
                    }

                }

            }
            catch (Exception)
            {
                return "error";
            }

        }

        public List<TeamMemebersEntity> GetTeamMembersByCompany(string companyName)
        {
            try
            {                
                return new TeamMemberHelper().GetTeamMembers(0, companyName, 0);//get all team members for a company
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int SaveCompanyTeamMembers(TeamMemebersEntity teamMembers)
        {
            try
            {
                teamMembers.SoftwareId = null;
                return new TeamMemberHelper().SaveTeamMember(teamMembers);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteCompanyTeamMember(int teamMemberId)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    return _context.Sp_DelTeamMember(teamMemberId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public TeamMemebersEntity GetCompanyTeamMember(int teamMemberId)
        {
            try
            {
                return new TeamMemberHelper().GetTeamMembers(teamMemberId, string.Empty, 0).FirstOrDefault();//get all team members for a company
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
