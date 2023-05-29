namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentAttendanceForExcelDto
    {
        public int Index { get; set; } = default!;
        public string Student { get; set; } = default!;
        public bool Present { get; set; } = default!;
        public DateTime DateOfAttendance { get; set; }
        public string CourseName { get; set; } = default!;
        public string LectureName { get; set; } = default!;
        public string Lecturer { get; set; } = default!;
        public string Notes { get; set; } = default!;
    }
}
