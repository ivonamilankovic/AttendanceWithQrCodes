using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class StudentInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Index { get; set; }

        [ForeignKey("UserId")]
        public int? UserId { get; set; }
        public User? User { get; set; } = default!;

        [ForeignKey("StudyProfileId")]
        public int? StudyProfileId { get; set; }
        public StudyProfile? StudyProfile { get; set; } 

        [ForeignKey("StudyLanguageId")]
        public int? StudyLanguageId { get; set; }
        public StudyLanguage? StudyLanguage { get; set; }

    }
}
