namespace AttendanceWithQrCodes.Models.Options
{
    public class LocationOptions
    {
        public double Latitude { get; set; } = default!;
        public double Longitude { get; set; } = default!;
        public double DeviationLat { get; set; } = default!;
        public double DeviationLong { get; set; } = default!;
    }
}
