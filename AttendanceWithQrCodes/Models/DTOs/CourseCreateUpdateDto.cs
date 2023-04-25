using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class CourseCreateUpdateDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public int LecturesNumForProfessor { get; set; } = default!;
        public int? LecturesNumForAssistent { get; set; }
        public int TotalTakenLectures { get; set; } = 0;
        [Required]
        public int ProfessorId { get; set; }
        public int? AssistantId { get; set; }
        [Required]
        [NotMapped]
        public IList<CourseLanguageIds> CourseLanguages { get; set; } = default!;
        [Required]
        [NotMapped]
        public IList<CourseStudyProfilesIds> CourseStudyProfiles { get; set; } = default!;
    }

    public class CourseLanguageIds
    {
        [Required]
        public int Id { get; set; } = default!;
    }
    public class CourseStudyProfilesIds
    {
        [Required]
        public int Id { get; set; } = default!;
    }

}
