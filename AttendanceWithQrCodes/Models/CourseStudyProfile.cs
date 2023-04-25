using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class CourseStudyProfile
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CourseId")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; } 

        [ForeignKey("StudyProfileId")]
        public int? StudyProfileId { get; set; }
        public StudyProfile? StudyProfile { get; set; } 
    }
}
