namespace AttendanceWithQrCodes.Models.DTOs
{
    public class UserDto
    {
        public int Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public RoleDto Role { get; set; } = default!;
    }
}
