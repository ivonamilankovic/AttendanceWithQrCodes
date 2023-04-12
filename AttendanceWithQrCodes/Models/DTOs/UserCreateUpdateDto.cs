using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models.DTOs
{
    public class UserCreateUpdateDto
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;
        [Required]
        public int RoleId { get; set; } = default!;
    }
}
