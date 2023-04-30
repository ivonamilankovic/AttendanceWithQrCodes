using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class CourseLanguage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CourseId")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("StudyLanguageId")]
        public int? StudyLanguageId { get; set; }
        public StudyLanguage? StudyLanguage { get; set; }
    }
}
