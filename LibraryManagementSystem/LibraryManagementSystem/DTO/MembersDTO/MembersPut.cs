namespace LibraryManagementSystem.DTO.MembersDTO
{
    public class MembersPut
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? ContactNumber { get; set; }

        public string? Address { get; set; }
    }
}
