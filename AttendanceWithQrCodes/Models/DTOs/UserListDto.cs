namespace AttendanceWithQrCodes.Models.DTOs
{
    public class UserListDto
    {
        public int Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public RoleDto Role { get; set; } = default!;
    }
}
