namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentInfoUpdateDto
    {
        public string MacAddress { get; set; } = default!;
        public int StudyProfileId { get; set; } = default!;
        public int StudyLanguageId { get; set; } = default!;
    }
}
