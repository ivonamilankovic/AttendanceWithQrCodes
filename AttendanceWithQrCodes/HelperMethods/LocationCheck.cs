using AttendanceWithQrCodes.Models.Options;
using Microsoft.Extensions.Options;

namespace AttendanceWithQrCodes.HelperMethods
{
    public class LocationCheck : ILocationCheck
    {
        private readonly LocationOptions _location;
        public LocationCheck(IOptions<LocationOptions> options)
        {
            _location = options.Value;
        }

        public bool IsLocationAcceptable(double latitude, double longitude)
        {
            bool goodLat = false;
            bool goodLong = false;

            double differenceLat = _location.Latitude - latitude;
            differenceLat = differenceLat < 0 ? differenceLat * (-1) : differenceLat;
            if(differenceLat < _location.DeviationLat)
            {
                goodLat = true;
            }

            double differenceLong = _location.Longitude - longitude;
            differenceLong = differenceLong < 0 ? differenceLong * (-1) : differenceLong;
            if(differenceLong < _location.DeviationLat)
            {
                goodLong = true;
            }

            return goodLat && goodLong;
        }

    }
}
