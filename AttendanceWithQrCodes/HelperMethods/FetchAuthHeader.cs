using Microsoft.Extensions.Primitives;

namespace AttendanceWithQrCodes.HelperMethods
{
    public class FetchAuthHeader : IFetchAuthHeader
    {
        public string FetchAuthorizationHeaderValue(HttpRequest request)
        {
            StringValues authValue;
            request.Headers.TryGetValue("Authorization", out authValue);
            string auth = (string)authValue;
            auth = auth.Substring(auth.IndexOf(' ') + 1);
            return auth;
        }
    }
}
