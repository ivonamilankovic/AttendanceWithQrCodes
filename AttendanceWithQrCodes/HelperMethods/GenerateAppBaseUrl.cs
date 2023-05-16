namespace AttendanceWithQrCodes.HelperMethods
{
    public class GenerateAppBaseUrl : IGenerateAppBaseUrl
    {
        private readonly HttpContext _httpContext;
        public GenerateAppBaseUrl(IHttpContextAccessor httpContextAccessor) 
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        /// Generates base app url with httpContext
        /// </summary>
        /// <returns>url string</returns>
        public string GetAppBaseUrl()
        {
            string baseUrl = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host.Value}";
            return baseUrl;
        }
    }
}
