namespace AttendanceWithQrCodes.Models.DTOs
{
    public class LectureDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime Date { get; set; }
        public CourseNameDto Course { get; set; } = default!;
        public UserNameEmailDto Lecturer { get; set; } = default!;
        public QrCodeDto QrCode { get; set; } = default!;
    }

}
