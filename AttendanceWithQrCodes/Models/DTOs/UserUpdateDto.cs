using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [NotMapped]
        public int RoleId { get; set; } = default!;
    }
}
