using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceWithQrCodes.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required] 
        public string Password { get; set; } = default!;

        [ForeignKey("RoleId")]
        public int? RoleId { get; set; }
        public Role? Role { get; set; } 
    }
}
