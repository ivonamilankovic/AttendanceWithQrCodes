using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    [Keyless]
    public class CourseLanguage
    {
        [ForeignKey("CourseId")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        [ForeignKey("StudyLanguageId")]
        public int? StudyLanguageId { get; set; }
        public StudyLanguage? StudyLanguage { get; set; }
    }
}
