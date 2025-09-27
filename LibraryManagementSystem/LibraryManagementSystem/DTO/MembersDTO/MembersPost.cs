namespace LibraryManagementSystem.DTO.MembersDTO
{
    public class MembersPost
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? ContactNumber { get; set; }

        public string? Address { get; set; }

        public DateOnly MembershipDate { get; set; }
    }
}
