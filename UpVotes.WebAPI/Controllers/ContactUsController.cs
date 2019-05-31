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
    //[AuthorizationRequired]
    public class ContactUsController : ApiController
    {        
        private readonly IContactUsService _contactUsServices;

        public ContactUsController(IContactUsService submitContactUsService)
        {
            _contactUsServices = submitContactUsService;
        }     
                
        [HttpPost]
        [Route("api/SaveContactUs")]
        public HttpResponseMessage SaveContactUsInformation(ContactUsInfoEntity contactrequest)
        {
            try
            {
                int ContactID = _contactUsServices.SaveContactUsInformation(contactrequest);
                if (ContactID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, ContactID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, ContactID);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        
    }
}
