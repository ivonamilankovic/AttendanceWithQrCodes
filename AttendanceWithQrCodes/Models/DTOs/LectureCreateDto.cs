using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class LectureCreateDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public int CourseId { get; set; } = default!;
        [Required]
        public int LecturerId { get; set; } = default!;
    }
}
