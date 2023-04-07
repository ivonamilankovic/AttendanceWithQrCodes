using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    [Keyless]
    public class CourseStudyProfile
    {
        [ForeignKey("CourseId")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; } 

        [ForeignKey("StudyProfileId")]
        public int? StudyProfileId { get; set; }
        public StudyProfile? StudyProfile { get; set; } 
    }
}
