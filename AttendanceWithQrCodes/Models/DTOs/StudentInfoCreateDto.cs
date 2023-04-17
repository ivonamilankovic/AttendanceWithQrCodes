using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentInfoCreateDto
    {
        [Required]
        public int Index { get; set; } = default!;
        [Required]
        public string MacAddress { get; set; } = default!;
        [Required]
        public int UserId { get; set; } = default!;
        [Required]
        public int StudyProfileId { get; set; } = default!;
        [Required]
        public int StudyLanguageId { get; set; } = default!;
    }
}
