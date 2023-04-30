namespace AttendanceWithQrCodes.Models.DTOs
{
    public class CourseDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public int LecturesNumForProfessor { get; set; } = default!;
        public int? LecturesNumForAssistent { get; set; }
        public int TotalTakenLectures { get; set; } = default!;
        public UserNameEmailDto Professor { get; set; } = default!;
        public UserNameEmailDto? Assistant { get; set; } = default!;
        public IList<CourseLanguageDto> Languages { get; set; } = default!;
        public IList<CourseStudyProfileDto> StudyProfiles { get; set; } = default!;

    }


}
