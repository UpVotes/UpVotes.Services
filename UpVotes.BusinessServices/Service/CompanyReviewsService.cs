using System;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.DataModel;
using UpVotes.DataModel.Utility;

namespace UpVotes.BusinessServices.Service
{
    public class CompanyReviewsService : ICompanyReviewsService
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
                    CompanyReview companyReview = new CompanyReview();
                    companyReview.CompanyID = companyReviewEntity.CompanyID;
                    companyReview.FocusAreaID = companyReviewEntity.FocusAreaID;
                    companyReview.UserID = companyReviewEntity.UserID;
                    companyReview.ReviewerCompanyName = companyReviewEntity.ReviewerCompanyName;
                    companyReview.Designation = companyReviewEntity.ReviewerDesignation;
                    companyReview.ProjectName = companyReviewEntity.ReviewerProjectName;
                    companyReview.FeedBack = companyReviewEntity.FeedBack;
                    companyReview.Rating = Convert.ToInt16(companyReviewEntity.Rating);
                    companyReview.ReviewDate = DateTime.Now;
                    companyReview.Email = companyReviewEntity.Email;
                    companyReview.Phone = companyReviewEntity.Phone;

                    _context.CompanyReviews.Add(companyReview);
                    _context.SaveChanges();
                    return true;                    
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
