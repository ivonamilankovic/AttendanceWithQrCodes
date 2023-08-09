namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentInfoDetailsDto
    {
        public int Index { get; set; } = default!;
        public UserNameEmailDto User { get; set; } = default!;
        public int? StudyProfileId { get; set; }
        public StudyProfileDto StudyProfile { get; set; } = default!;
        public int? StudyLanguageId { get; set; }
        public StudyLanguageDto StudyLanguage { get; set; } = default!;
    }
}
