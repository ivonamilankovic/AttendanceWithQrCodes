using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class UserChangePasswordDto
    {
        [Required]
        public string Password { get; set; } = default!;
    }
}
