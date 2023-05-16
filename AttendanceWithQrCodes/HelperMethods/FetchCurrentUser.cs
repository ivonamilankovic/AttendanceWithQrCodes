using System.Security.Claims;

namespace AttendanceWithQrCodes.HelperMethods
{
    public class FetchCurrentUser : IFetchCurrentUser
    {
        private readonly HttpContext _httpContext;

        public FetchCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// Geta user id from HttpContext Claims Identity
        /// </summary>
        /// <returns>user id</returns>
        public int GetCurrentUserId()
        {
            ClaimsIdentity? identity = _httpContext.User.Identity as ClaimsIdentity;
            int id = 0;
            if (identity != null)
            {
                IEnumerable<Claim> userClaims = identity.Claims;
                id = int.Parse(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.UserData).Value);
            }
            return id;
        }
    }
}
