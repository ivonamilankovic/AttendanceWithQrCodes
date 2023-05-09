namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentAttendanceDetailsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Present { get; set; } = default!;
        public StudentInfoDetailsDto Student { get; set; } = default!;
        public LectureDetailsDto Lecture { get; set; } = default!;
    }
}
