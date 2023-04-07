using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models
{
    public class StudyLanguage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
    }
}
