namespace AttendanceWithQrCodes.Models.DTOs
{
    public class CourseListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public UserNameEmailDto Professor { get; set; } = default!;
        public UserNameEmailDto? Assistant { get; set; } = default!;
    }
}
