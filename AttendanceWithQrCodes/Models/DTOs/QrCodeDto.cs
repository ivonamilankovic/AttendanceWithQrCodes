namespace AttendanceWithQrCodes.Models.DTOs
{
    public class QrCodeDto
    {
        public string Data { get; set; } = default!;
        public string ImageName { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }
}
