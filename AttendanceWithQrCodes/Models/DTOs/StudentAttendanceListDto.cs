namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentAttendanceListDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; } = default!;
        public string? Notes { get; set; } = default!;
        public StudentInfoAttendanceDetailsDto Student { get; set; } = default!;
        public LectureAttendanceDetailsDto Lecture { get; set; } = default!;
    }

    public class StudentInfoAttendanceDetailsDto
    {
        public int Index { get; set; } = default!;
        public UserNameEmailDto User { get; set; } = default!;
    }
    public class LectureAttendanceDetailsDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Date { get; set; }
        public CourseNameDto Course { get; set; } = default!;
        public UserNameEmailDto Lecturer { get; set; } = default!;
    }
}
