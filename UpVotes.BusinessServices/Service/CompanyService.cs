using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.UnitOfWork;
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

        private readonly UnitOfWork _unitOfWork;

        public CompanyService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
            UpVotesEntities upVotesObj = new UpVotesEntities();
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

        public CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, int userID = 0)
        {
            CompanyDetail companyDetail = new CompanyDetail();
            companyDetail.CompanyList = new List<CompanyEntity>();
            UpVotesEntities upVotesObj = new UpVotesEntities();
            try
            {
                IEnumerable<CompanyEntity> companyEntities = GetCompany(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID, userID);
                if (companyEntities.Count() > 0)
                {
                    foreach (CompanyEntity company in companyEntities)
                    {
                        if (companyName != "0")
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
            SqlParameter p1 = new SqlParameter("@companyName", System.Data.SqlDbType.Int) { Value = companyName };

            IEnumerable<Sp_GetCompany_Result> company = _unitOfWork.SpGetCompanyResults.ExecWithStoreProcedure("EXEC Sp_GetCompany @companyName", p1);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompany_Result, CompanyEntity>(); });
            IEnumerable<CompanyEntity> companyEntity = Mapper.Map<IEnumerable<Sp_GetCompany_Result>, IEnumerable<CompanyEntity>>(company);
            return companyEntity;
        }

        private IEnumerable<CompanyEntity> GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, int userID = 0)
        {
            SqlParameter p1 = new SqlParameter("@companyName", System.Data.SqlDbType.NVarChar) { Value = companyName };
            SqlParameter p2 = new SqlParameter("@minRate", System.Data.SqlDbType.Decimal) { Value = minRate };
            SqlParameter p3 = new SqlParameter("@maxRate", System.Data.SqlDbType.Decimal) { Value = maxRate };
            SqlParameter p4 = new SqlParameter("@minEmployee", System.Data.SqlDbType.Int) { Value = minEmployee };
            SqlParameter p5 = new SqlParameter("@maxEmployee", System.Data.SqlDbType.Int) { Value = maxEmployee };
            SqlParameter p6 = new SqlParameter("@sortByVotes", System.Data.SqlDbType.NVarChar) { Value = sortby };
            SqlParameter p7 = new SqlParameter("@focusAreaID", System.Data.SqlDbType.Int) { Value = focusAreaID };
            SqlParameter p8 = new SqlParameter("@userID", System.Data.SqlDbType.Int) { Value = userID };

            IEnumerable<Sp_GetCompany_Result> company = _unitOfWork.SpGetCompanyResults.ExecWithStoreProcedure("EXEC Sp_GetCompany @companyName, @minRate, @maxRate, @minEmployee, @maxEmployee, @sortByVotes, @focusAreaID, @userID", p1, p2, p3, p4, p5, p6, p7, p8);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompany_Result, CompanyEntity>(); });
            IEnumerable<CompanyEntity> companyEntity = Mapper.Map<IEnumerable<Sp_GetCompany_Result>, IEnumerable<CompanyEntity>>(company);

            return companyEntity;
        }

        private IEnumerable<CompanyPortFolioEntity> GetCompanyPortFolio(int companyID)
        {
            SqlParameter p1 = new SqlParameter("@CompanyID", System.Data.SqlDbType.Int) { Value = companyID };

            IEnumerable<Sp_GetCompanyPortFolio_Result> companyPortFolio = _unitOfWork.SpGetCompanyPortFolio.ExecWithStoreProcedure("Sp_GetCompanyPortFolio @CompanyID", p1);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyPortFolio_Result, CompanyPortFolioEntity>(); });
            IEnumerable<CompanyPortFolioEntity> companyPortFolioEntity = Mapper.Map<IEnumerable<Sp_GetCompanyPortFolio_Result>, IEnumerable<CompanyPortFolioEntity>>(companyPortFolio);
            return companyPortFolioEntity;
        }

        private IEnumerable<CompanyFocusEntity> GetCompanyFocus(int companyID)
        {
            SqlParameter p1 = new SqlParameter("@CompanyID", System.Data.SqlDbType.Int) { Value = companyID };

            IEnumerable<Sp_GetCompanyFocus_Result> companyFocus = _unitOfWork.SpGetCompanyFocus.ExecWithStoreProcedure("Sp_GetCompanyFocus @CompanyID", p1);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyFocus_Result, CompanyFocusEntity>(); });
            IEnumerable<CompanyFocusEntity> companyFocusEntity = Mapper.Map<IEnumerable<Sp_GetCompanyFocus_Result>, IEnumerable<CompanyFocusEntity>>(companyFocus);
            return companyFocusEntity;
        }

        private IEnumerable<CompanyBranchEntity> GetCompanyBranches(int companyID)
        {
            SqlParameter p1 = new SqlParameter("@CompanyID", System.Data.SqlDbType.Int) { Value = companyID };

            IEnumerable<Sp_GetCompanyBranches_Result> companyBranches = _unitOfWork.SpGetCompanyBranches.ExecWithStoreProcedure("Sp_GetCompanyBranches @CompanyID", p1);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyBranches_Result, CompanyBranchEntity>(); });
            IEnumerable<CompanyBranchEntity> companyBranchEntity = Mapper.Map<IEnumerable<Sp_GetCompanyBranches_Result>, IEnumerable<CompanyBranchEntity>>(companyBranches);
            return companyBranchEntity;
        }

        private IEnumerable<CompanyReviewsEntity> GetCompanyReviews(int companyID)
        {
            SqlParameter p1 = new SqlParameter("@CompanyID", System.Data.SqlDbType.Int) { Value = companyID };

            IEnumerable<Sp_GetCompanyReviews_Result> companyReviews = _unitOfWork.SpGetCompanyReviews.ExecWithStoreProcedure("Sp_GetCompanyReviews @CompanyID", p1);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyReviews_Result, CompanyReviewsEntity>(); });
            IEnumerable<CompanyReviewsEntity> companyReviewEntity = Mapper.Map<IEnumerable<Sp_GetCompanyReviews_Result>, IEnumerable<CompanyReviewsEntity>>(companyReviews);
            return companyReviewEntity;
        }

        private IEnumerable<CompanyReviewThankNoteEntity> GetCompanyReviewThankNotes(int companyID, int companyReviewID)
        {
            SqlParameter p1 = new SqlParameter("@CompanyID", System.Data.SqlDbType.Int) { Value = companyID };
            SqlParameter p2 = new SqlParameter("@CompanyReviewID", System.Data.SqlDbType.Int) { Value = companyReviewID };

            IEnumerable<Sp_GetCompanyReviewThankNotedUsers_Result> companyReviewThankNotes = _unitOfWork.SpGetCompanyReviewThankNotedUsers.ExecWithStoreProcedure("Sp_GetCompanyReviewThankNotedUsers @CompanyID, @CompanyReviewID", p1, p2);
            Mapper.Initialize(cfg => { cfg.CreateMap<Sp_GetCompanyReviewThankNotedUsers_Result, CompanyReviewThankNoteEntity>(); });
            IEnumerable<CompanyReviewThankNoteEntity> companyReviewThankNotesEntity = Mapper.Map<IEnumerable<Sp_GetCompanyReviewThankNotedUsers_Result>, IEnumerable<CompanyReviewThankNoteEntity>>(companyReviewThankNotes);
            return companyReviewThankNotesEntity;
        }

        //private IEnumerable<CompanyReviewThankNoteEntity> GetCompanyReviewThankNotes(int companyID, int companyReviewID)
        //{
        //    Func<VwGetCompanyReviewThankNotedUsers, bool> companyReviewThankNotesWhere = e => e.CompanyID == companyID && e.CompanyReviewID == companyReviewID;
        //    IEnumerable<VwGetCompanyReviewThankNotedUsers> companyReviewThankNotes = _unitOfWork.VwGetCompanyReviewThankNotedUsers.GetMany(companyReviewThankNotesWhere);
        //    Mapper.Initialize(cfg => { cfg.CreateMap<VwGetCompanyReviewThankNotedUsers, CompanyReviewThankNoteEntity>(); });
        //    IEnumerable<CompanyReviewThankNoteEntity> companyReviewThankNotesEntity = Mapper.Map<IEnumerable<VwGetCompanyReviewThankNotedUsers>, IEnumerable<CompanyReviewThankNoteEntity>>(companyReviewThankNotes);
        //    return companyReviewThankNotesEntity;
        //}

        public IEnumerable<AutoComplete> GetCompanyNames()
        {
            var companies = (from a in _unitOfWork.CompanyRepository.GetAll()
                             select new AutoComplete()
                             {
                                 ID = a.CompanyID,
                                 Value = a.CompanyName
                             });

            return companies;

        }

        public string VoteForCompany(CompanyVoteEntity companyVote)
        {
            try
            {
                Func<CompanyVote, bool> companyVoteWhere = e => e.CompanyID == companyVote.CompanyID && e.UserID == companyVote.UserID;
                bool isVoted = _unitOfWork.CompanyVoteRepository.Get(companyVoteWhere) == null ? false : true;
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

                    bool isSuccess = _unitOfWork.CompanyVoteRepository.Add(companyVoteAdd);
                    _unitOfWork.Save();

                    if(isSuccess)
                    {
                        return "Thanks for voting.";
                    }
                    else
                    {
                        return "Something error occured while voting. Please contact support.";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ThanksNoteForReview(CompanyReviewThankNoteEntity companyReviewThanksNoteEntity)
        {
            try
            {
                Func<CompanyReviewThankNote, bool> companyReviewThankNoteWhere = e => e.CompanyReview.CompanyID == companyReviewThanksNoteEntity.CompanyID && e.UserID == companyReviewThanksNoteEntity.UserID && e.CompanyReviewID == companyReviewThanksNoteEntity.CompanyReviewID;
                bool isThanksNoted = _unitOfWork.CompanyReviewThankNote.Get(companyReviewThankNoteWhere) == null ? false : true;
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

                    bool isSuccess = _unitOfWork.CompanyReviewThankNote.Add(companyReviewThankNoteAdd);
                    _unitOfWork.Save();

                    if (isSuccess)
                    {
                        return "Your thanks note is recorded for this review.";
                    }
                    else
                    {
                        return "Something error occured while providing thanks note. Please contact support.";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
