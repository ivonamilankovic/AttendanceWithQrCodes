using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class CourseListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public UserNameEmailDto Professor { get; set; } = default!;
        public UserNameEmailDto? Assistant { get; set; } = default!;
        [Required]
        public int LecturesNumForProfessor { get; set; } = default!;
        public int? LecturesNumForAssistent { get; set; }
        [Required]
        public int TotalTakenLectures { get; set; } = default!;
    }
}
