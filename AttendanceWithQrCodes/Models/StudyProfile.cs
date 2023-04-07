using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models
{
    public class StudyProfile
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string Name { get; set; } = default!;
    }
}
