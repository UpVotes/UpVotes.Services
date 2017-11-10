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
    public class FocusAreaController : ApiController
    {
        private readonly IFocusAreaService _focusAreaService;

        public FocusAreaController(IFocusAreaService focusAreaService)
        {
            _focusAreaService = focusAreaService;
        }

        [HttpGet]
        [Route("api/GetFocusAreas")]
        public HttpResponseMessage GetFocusAreas()
        {
            try
            {
                List<FocusAreaEntity> focusArea = _focusAreaService.GetFocusAreaList();
                if (focusArea.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, focusArea);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Focus Area not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("api/GetFocusAreaID/{focusAreaName}")]
        public HttpResponseMessage GetFocusAreaID(string focusAreaName)
        {
            try
            {
                int focusAreaID = _focusAreaService.GetFocusAreaID(focusAreaName);
                if (focusAreaID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, focusAreaID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Focus Area not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
