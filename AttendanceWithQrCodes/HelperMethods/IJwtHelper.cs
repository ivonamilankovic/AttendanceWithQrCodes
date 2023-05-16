using AttendanceWithQrCodes.Models;

namespace AttendanceWithQrCodes.HelperMethods
{
    public interface IJwtHelper
    {
        public string GenerateLoginToken(User user);
        public int ValidateLoginToken(string token);
    }
}