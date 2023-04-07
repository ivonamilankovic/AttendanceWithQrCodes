using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class Lecture
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public DateTime Date { get; set; }

        [ForeignKey("CourseId")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; } 

        [ForeignKey("LecturerId")]
        public int? LecturerId { get; set; }
        public User? Lecturer { get; set; } 

        [ForeignKey("QrCodeId")]
        public int? QrCodeId { get; set; }
        public QrCode? QrCode { get; set; } 
    }
}
