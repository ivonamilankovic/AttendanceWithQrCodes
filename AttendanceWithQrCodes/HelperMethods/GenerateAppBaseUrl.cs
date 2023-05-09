namespace AttendanceWithQrCodes.HelperMethods
{
    public class GenerateAppBaseUrl : IGenerateAppBaseUrl
    {
        private readonly HttpContext _httpContext;
        public GenerateAppBaseUrl(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string GetAppBaseUrl()
        {
            string baseUrl = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host.Value}";
            return baseUrl;
        }
    }
}
