using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class LectureUpdateDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
    }
}
