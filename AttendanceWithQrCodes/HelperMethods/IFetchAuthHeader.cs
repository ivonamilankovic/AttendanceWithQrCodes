namespace AttendanceWithQrCodes.HelperMethods
{
    public interface IFetchAuthHeader
    {
        public string FetchAuthorizationHeaderValue(HttpRequest request);
    }
}