namespace AttendanceWithQrCodes.HelperMethods
{
    public interface ILocationCheck
    {
        public bool IsLocationAcceptable(double latitude, double longitude);
    }
}