using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
                        company.CompanyReviews = GetCompanyReviews(company.CompanyID).ToList();
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

        public CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID,string location, int userID = 0)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();
            
            try
            {
                IEnumerable<CompanyEntity> companyEntities = GetCompany(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID,location, userID);
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
                            company.CompanyReviews = GetCompanyReviews(company.CompanyID).ToList();
                            if (company.CompanyReviews.Count() > 0)
                            {
                                foreach (CompanyReviewsEntity companyReviewsEntity in company.CompanyReviews)
                                {
                                    companyReviewsEntity.NoOfThankNotes = GetCompanyReviewThankNotes(company.CompanyID, companyReviewsEntity.CompanyReviewID).Count();
                                }

                                company.UserRating = Convert.ToInt32(company.CompanyReviews.Average(i => i.Rating));
                                company.NoOfUsersRated = company.CompanyReviews.Count();
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

        private IEnumerable<CompanyEntity> GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID,string location, int userID = 0)
        {
            using (_context = new UpVotesEntities())
            {
                string sqlQuery = "EXEC Sp_GetCompany '" + companyName + "'," + minRate + "," + maxRate + "," + minEmployee + "," + maxEmployee + ",'" + sortby + "'," + focusAreaID + "," + userID + "," + location;
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

        private IEnumerable<CompanyReviewsEntity> GetCompanyReviews(int companyID)
        {
            using (_context = new UpVotesEntities())
            {
                IEnumerable<Sp_GetCompanyReviews_Result> companyReviews = _context.Database.SqlQuery(typeof(Sp_GetCompanyReviews_Result), "EXEC Sp_GetCompanyReviews " + companyID).Cast<Sp_GetCompanyReviews_Result>().AsEnumerable();
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

        public IEnumerable<AutoComplete> GetCompanyNames()
        {
            //var companies = (from a in _unitOfWork.CompanyRepository.GetAll()
            //                 select new AutoComplete()
            //                 {
            //                     ID = a.CompanyID,
            //                     Value = a.CompanyName
            //                 });

            //return companies;

            return null;
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
    }
}
