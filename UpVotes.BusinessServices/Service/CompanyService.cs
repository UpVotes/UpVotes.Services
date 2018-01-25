using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using UpVotes.BusinessEntities.Entities;
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

        public bool InsertCompany(CompanyEntity companyEntity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCompany(int companyID)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompany(int companyID, CompanyEntity companyEntity)
        {
            throw new NotImplementedException();
        }

        public CompanyDetail GetCompanyDetails(string companyName)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            try
            {
                IEnumerable<CompanyEntity> companyEntities = GetCompany(companyName);
                if (companyEntities.Count() > 0)
                {
                    foreach (CompanyEntity company in companyEntities)
                    {
                        company.CompanyFocus = GetCompanyFocus(company.CompanyID).ToList();
                        company.CompanyBranches = GetCompanyBranches(company.CompanyID).ToList();
                        company.CompanyPortFolio = GetCompanyPortFolio(company.CompanyID).ToList();
                        company.CompanyReviews = GetCompanyReviews(company.CompanyName).ToList();
                        if (company.CompanyReviews.Count() > 0)
                        {
                            foreach (CompanyReviewsEntity companyReviewsEntity in company.CompanyReviews)
                            {
                                companyReviewsEntity.NoOfThankNotes = GetCompanyReviewThankNotes(company.CompanyID, companyReviewsEntity.CompanyReviewID).Count();
                            }

                            company.UserRating = Convert.ToInt32(company.CompanyReviews.Average(i => i.Rating));
                            company.NoOfUsersRated = company.CompanyReviews.Count();
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

        public CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, int userID = 0, int PageNo = 1, int PageSize = 10)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            try
            {
                IEnumerable<CompanyEntity> companyEntities = GetCompany(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID, location, userID, PageNo, PageSize);
                if (companyEntities.Count() > 0)
                {
                    foreach (CompanyEntity company in companyEntities)
                    {
                        if (companyName != "0")
                        {
                            company.CompanyName = System.Web.HttpUtility.HtmlEncode(company.CompanyName);
                            company.CompanyFocus = GetCompanyFocus(company.CompanyID).ToList();
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

        private IEnumerable<CompanyEntity> GetCompany(string companyName)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompany_Result> company = _context.Database.SqlQuery(typeof(Sp_GetCompany_Result), "EXEC Sp_GetCompany '" + companyName + "'").Cast<Sp_GetCompany_Result>().AsEnumerable();
                Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompany_Result, CompanyEntity>(); });
                IEnumerable<CompanyEntity> companyEntity = Mapper.Map<IEnumerable<Sp_GetCompany_Result>, IEnumerable<CompanyEntity>>(company);
                return companyEntity;
            }
        }

        private IEnumerable<CompanyEntity> GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, int userID = 0, int PageNo = 1, int PageSize = 10)
        {
            using (_context = new UpVotesEntities())
            {
                string sqlQuery = "EXEC Sp_GetCompany '" + companyName + "'," + minRate + "," + maxRate + "," + minEmployee + "," + maxEmployee + ",'" + sortby + "'," + focusAreaID + "," + userID + "," + location + "," + PageNo + "," + PageSize;
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
                        SendEmailForInternalUse(user, companyName);

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

        private void SendEmailForInternalUse(User user, string companyName)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                MailAddress mailAddress = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);
                var From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);
                var To = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailTo"]);
                using (MailMessage message = new MailMessage(From, To))
                {
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = user.FirstName + " " + user.LastName + " voted for " + companyName;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.Priority = MailPriority.Normal;
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"], System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"]);

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<p>Hello,</p><p> We got a vote for&nbsp;<em><strong><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "profile/" + companyName.Replace(" ", "-") + "' target = '_blank' rel = 'noopener'>" + companyName + "</a> &nbsp;</strong></em> from &nbsp;<em><strong>"
                        + user.FirstName + " " + user.LastName + ".</strong></em></p>");
                    sb.Append("<p> Click <a href = '" + user.ProfileURL + "' target = '_blank' rel = 'noopener' > here </a> to know more about user.</p>");
                    sb.Append("<p><a href = '" + user.ProfileURL + "' ><img src = '" + user.ProfilePictureURL + "' alt = '" + user.FirstName + " " + user.LastName + "' width = '80' height = '80' /></a></p>");
                    sb.Append("<p><strong> Thanks & Regards </strong></p>");
                    sb.Append("<p> Upvotes.Co </p>");
                    sb.Append("<p><a href = 'mailto:" + System.Configuration.ConfigurationManager.AppSettings["AdminEmail"] + "' > support@upvotes.co </a></p>");
                    sb.Append("<p><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "' > www.upvotes.co </a></p>");

                    message.Body = sb.ToString();

                    smtpClient.Host = "smtpout.secureserver.net";
                    smtpClient.Port = 80;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = false;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"], System.Configuration.ConfigurationManager.AppSettings["AdminPassword"]);
                    smtpClient.Credentials = credentials;
                    smtpClient.Send(message);
                }
            }
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
                        myAutoCompleteList = (from a in _context.Company
                                              join b in _context.CompanyFocus on a.CompanyID equals b.CompanyID
                                              where a.IsActive == true && b.FocusAreaID == focusAreaID && a.CompanyName.Trim().ToUpper().Contains(searchTerm.Trim().ToUpper())
                                              orderby a.CompanyName
                                              select a.CompanyName).Distinct().ToList();

                        return myAutoCompleteList;
                    }
                    else if (type == 2)//Location
                    {
                        myAutoCompleteList = (from a in _context.Company
                                              join b in _context.CompanyFocus on a.CompanyID equals b.CompanyID
                                              join c in _context.CompanyBranches on a.CompanyID equals c.CompanyID
                                              where a.IsActive == true && b.FocusAreaID == focusAreaID && c.IsActive == true && c.City.Trim().ToUpper().Contains(searchTerm.Trim().ToUpper())
                                              orderby c.City
                                              select c.City).Distinct().ToList();

                        if (myAutoCompleteList.Any())
                        {
                            return myAutoCompleteList;
                        }
                        else
                        {
                            myAutoCompleteList = (from a in _context.Company
                                                  join b in _context.CompanyFocus on a.CompanyID equals b.CompanyID
                                                  join c in _context.CompanyBranches on a.CompanyID equals c.CompanyID
                                                  join d in _context.States on c.StateID equals d.StateID
                                                  where a.IsActive == true && c.IsActive == true && d.StateName.Trim().ToUpper().Contains(searchTerm.Trim().ToUpper())
                                                  orderby d.StateName
                                                  select d.StateName).Distinct().ToList();

                            if (myAutoCompleteList.Any())
                            {
                                return myAutoCompleteList;
                            }
                            else
                            {
                                myAutoCompleteList = (from a in _context.Company
                                                      join b in _context.CompanyFocus on a.CompanyID equals b.CompanyID
                                                      join c in _context.CompanyBranches on a.CompanyID equals c.CompanyID
                                                      join d in _context.Countries on c.CountryID equals d.CountryID
                                                      where a.IsActive == true && c.IsActive == true && d.CountryName.Trim().ToUpper().Contains(searchTerm.Trim().ToUpper())
                                                      orderby d.CountryName
                                                      select d.CountryName).Distinct().ToList();

                                return myAutoCompleteList;
                            }
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

        public CompanyDetail GetUserReviews(CompanyEntity companyEntity)
        {
            string[] companyNamesList = companyEntity.CompanyName.Split(',');
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            foreach (string companyName in companyNamesList)
            {

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

            }

            return companyDetail;
        }

    }
}
