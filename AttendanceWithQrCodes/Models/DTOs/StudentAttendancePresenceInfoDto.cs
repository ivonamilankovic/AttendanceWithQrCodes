namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentAttendancePresenceInfoDto
    {
        public int TotalNeededLectures { get; set; } = default!;
        public int TotalTakenLectures { get; set; } = default!;
        public int TotalPresentLectures { get; set; } = default!;
        public double AttendancePercentage { get; set; } = default!;
    }
}
