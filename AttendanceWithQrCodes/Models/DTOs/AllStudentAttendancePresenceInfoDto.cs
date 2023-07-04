namespace AttendanceWithQrCodes.Models.DTOs
{
    public class AllStudentAttendancePresenceInfoDto
    {
        public int TotalNeededLectures { get; set; } = default!;
        public int TotalTakenLectures { get; set; } = default!;
        public int TotalStudentsForCourse { get; set; } = default!;
        public int TotalPresentStudentsForCourse { get; set; } = default!;
        public double PercentageOfStudents { get; set; } = default!; //Students Who Attend More Than Half Taken Lectures
    }
}
