using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class StudentAttendance
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool Present { get; set; } = default;

        [ForeignKey("StudentIndex")]
        public int? StudentIndex { get; set; }
        public StudentInformation? StudentInformation { get; set; }

        [ForeignKey("LectureId")]
        public int? LectureId { get; set; }
        public Lecture? Lecture { get; set; }
    }
}
