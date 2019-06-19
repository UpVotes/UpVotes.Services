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
    public class SponsorshipService : ISponsorshipService
    {
        private static Logger Log => Logger.Instance();

        private UpVotesEntities _context = null;

        public bool InsertUpdateSponsorShip(SponsorshipRequestEntity sponsorshipObj)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    var sponsored = _context.Sp_InsUpdServiceSoftwareSponsorship(sponsorshipObj.Provider, sponsorshipObj.CompanyOrSoftwareID, sponsorshipObj.SponsorshipCategoryID, sponsorshipObj.CreatedBy, Convert.ToDateTime(sponsorshipObj.StartDate), Convert.ToDateTime(sponsorshipObj.EndDate)).FirstOrDefault();
                    if (Convert.ToInt32(sponsored) > 0)
                    {
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

        public bool ScheduleForDeactivateSponsor(SponsorshipRequestEntity sponsorshipObj)
        {
            try
            {
                using (_context = new UpVotesEntities())
                {
                    _context.Sp_SchedulerForExpiredServiceSoftwareSponsorshipList(sponsorshipObj.CreatedBy);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SponsorshipExpiredListEntity> GetExpiredSponsorshipList(SponsorshipRequestEntity requestObj)
        {
            List<SponsorshipExpiredListEntity> ExpiredListObj = new List<SponsorshipExpiredListEntity>();
            ExpiredListObj = GetAllExpiredSponsorshipList(requestObj).ToList();
            return ExpiredListObj;
        }

        public List<SponsorshipExpiredListEntity> GetAllExpiredSponsorshipList(SponsorshipRequestEntity requestObj)
        {
            List<SponsorshipExpiredListEntity> ExpiredListObj = new List<SponsorshipExpiredListEntity>(); 
            using (_context = new UpVotesEntities())
            {
                var response = _context.Sp_GetExpiredServiceSoftwareSponsorshipList(requestObj.CreatedBy).ToList();
                if (response != null)
                {
                    response.ToList().ForEach(q => ExpiredListObj.Add(new SponsorshipExpiredListEntity
                    {
                        Category = q.Category,
                        Name = q.Name,
                        Package = q.Package
                    }
                ));
                }
            }
            return ExpiredListObj;
        }
    }
}
