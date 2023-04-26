using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public int LecturesNumForProfessor { get; set; } = default!;
        public int? LecturesNumForAssistent { get; set; }
        [Required]
        public int TotalTakenLectures { get; set; } = default!;

        [ForeignKey("ProfessorId")]  
        public int?  ProfessorId { get; set; }
        public User? Professor { get; set; } 

        [ForeignKey("AssistantId")]
        public int? AssistantId { get; set; }
        public User? Assistant { get; set; }

        [NotMapped]
        public IList<CourseLanguage>? CourseLanguages { get; set; }
        [NotMapped]
        public IList<CourseStudyProfile>? CourseStudyProfiles { get; set; } 

    }
}
