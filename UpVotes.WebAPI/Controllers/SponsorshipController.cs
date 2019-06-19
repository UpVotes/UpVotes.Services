using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;

namespace UpVotes.WebAPI.Controllers
{
    public class SponsorshipController : ApiController
    {
        private readonly ISponsorshipService _sponsorServices;

        public SponsorshipController(ISponsorshipService sponsorService)
        {
            _sponsorServices = sponsorService;
        }

        [HttpPost, Route("api/ApplySponsorShip")]
        public HttpResponseMessage InsertUpdateSponsorShip(SponsorshipRequestEntity sponsorshipRequestObj)
        {
            try
            {
                bool isSponsored = _sponsorServices.InsertUpdateSponsorShip(sponsorshipRequestObj);
                return Request.CreateResponse(HttpStatusCode.OK, isSponsored);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/GetAllExpiredSponsorshipList")]
        public HttpResponseMessage GetExpiredSponsorshipList(SponsorshipRequestEntity sponsorshipRequestObj)
        {
            try
            {
                List<SponsorshipExpiredListEntity> ExpiredList = _sponsorServices.GetExpiredSponsorshipList(sponsorshipRequestObj);
                return Request.CreateResponse(HttpStatusCode.OK, ExpiredList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/SchedulerToDeactivateSponsor")]
        public HttpResponseMessage ScheduleForDeactivateSponsor(SponsorshipRequestEntity sponsorshipRequestObj)
        {
            try
            {
                bool isScheduled = _sponsorServices.ScheduleForDeactivateSponsor(sponsorshipRequestObj);
                return Request.CreateResponse(HttpStatusCode.OK, isScheduled);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
