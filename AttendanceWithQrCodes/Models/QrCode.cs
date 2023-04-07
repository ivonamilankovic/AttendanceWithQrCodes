using System.ComponentModel.DataAnnotations;

namespace AttendanceWithQrCodes.Models
{
    public class QrCode
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Data { get; set; } = default!;
        [Required]
        public string ImageName { get; set; } = default!;
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
