namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentInfoDetailsDto
    {
        public int Index { get; set; } = default!;
        public string MacAddress { get; set; } = default!;
        public UserNameEmailDto User { get; set; } = default!;
        public StudyProfileDto StudyProfile { get; set; } = default!;
        public StudyLanguageDto StudyLanguage { get; set; } = default!;
    }
}
