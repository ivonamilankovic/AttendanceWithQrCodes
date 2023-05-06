using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class StudentAttendanceDto
    {
        [Required]
        public int LectureId { get; set; } = default!;
        [Required]
        public int Index { get; set; } = default!;
        [Required]
        [NotMapped]
        public string MacAddress { get; set; } = default!;
    }
}
