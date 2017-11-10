using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UpVotes.BusinessServices.Interface;
using UpVotes.WebAPI.Filters;
using UpVotes.WebAPI.Filters.Authentication;

namespace UpVotes.WebAPI.Controllers
{
    [ApiAuthenticationFilter]
    public class AuthenticateController : ApiController
    {
        private readonly IUserTokenService _tokenServices;

        public AuthenticateController(IUserTokenService tokenServices)
        {
            _tokenServices = tokenServices;
        }

        [HttpPost]
        [Route("api/GetAuthToken")]
        public HttpResponseMessage GetAuthToken()
        {
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {
                    var userId = basicAuthenticationIdentity.UserId;
                    return GetAuthToken(userId);
                }
            }
            return null;
        }

        private HttpResponseMessage GetAuthToken(int userId)
        {
            var token = _tokenServices.GenerateToken(userId);
            var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
            response.Headers.Add("Token", token.AuthToken);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }
    }
}