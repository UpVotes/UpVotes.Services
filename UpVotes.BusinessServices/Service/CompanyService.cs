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

        private void SendEmailForQuotation(QuotationRequest request,QuotationResponse response)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                MailAddress mailAddress = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);
                var From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"]);
                var To = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["EmailTo"]);
                var UserTo = new MailAddress(request.EmailId);
                using (MailMessage message = new MailMessage(From, UserTo))
                {
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = "Your Mobile App Development Cost Estimate";
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.Priority = MailPriority.Normal;
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"], System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"]);

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<p>Dear "+ request.Name + ",</p><p> Thanks for trying out the Mobile App Cost Estimator. You can always find the details of your estimate by table below.</p>");
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

                   
                    sb.Append("<p><strong> Thanks & Regards </strong></p>");
                    sb.Append("<p>Upvotes team</p>");
                    sb.Append("<p><a href = '" + System.Configuration.ConfigurationManager.AppSettings["WebClientURL"] + "' > www.upvotes.co </a></p>");
                    sb.Append("<p>Follow us on - <a href = 'https://www.linkedin.com/company/upvotes/'> LinkedIn </a> &nbsp;&nbsp;|&nbsp;&nbsp; <a href = 'https://twitter.com/upvotes_co'> Twitter </a> &nbsp;&nbsp;|&nbsp;&nbsp;<a href = 'https://www.facebook.com/upvotes.co/'> Facebook </a></p>");

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

                using (MailMessage message = new MailMessage(From, To))
                {
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.Subject = request.Name + " - Mobile App Cost Estimator ";
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.Priority = MailPriority.Normal;
                    message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["AdminEmail"], System.Configuration.ConfigurationManager.AppSettings["DomainDisplayName"]);

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("<p>Hello,</p><p> " + request.Name + "(" + request.EmailId + ")" + " is used Upvotes Quotation tool");
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
                            myAutoCompleteList = _context.Database.SqlQuery(typeof(string), "EXEC Sp_GetCompanyNames " + type + "," + focusAreaID+","+ searchTerm).Cast<string>().ToList();

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

        public CompanyDetail GetUserCompanies(int userID)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();

            try
            {
                using (_context = new UpVotesEntities())
                {
                    List<Company> companyListDb = _context.Company.Where(a => a.CreatedBy == userID).Take(2).ToList();
                    foreach (Company companyDb in companyListDb)
                    {
                        CompanyEntity companyEntity = new CompanyEntity
                        {
                            CompanyID = companyDb.CompanyID,
                            CompanyName = companyDb.CompanyName,
                            UserRating = companyDb.CompanyReviews.Count() > 0 ? Convert.ToInt32(companyDb.CompanyReviews.Average(a => a.Rating)) : 0,
                            NoOfVotes = companyDb.CompanyVote.Count() > 0 ? companyDb.CompanyVote.Count() : 0,
                            FoundedYear = companyDb.FoundedYear,
                            URL = companyDb.URL
                        };
                        companyDetail.CompanyList.Add(companyEntity);
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

    }
}
