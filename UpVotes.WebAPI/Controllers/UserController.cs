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

    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("api/AddOrUpdateUser")]
        public HttpResponseMessage AddOrUpdateUser(UserEntity userObj)
        {
            try
            {
                UserEntity updatedUserObj = _userService.AddOrUpdateUser(userObj);
                return Request.CreateResponse(HttpStatusCode.OK, updatedUserObj);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
