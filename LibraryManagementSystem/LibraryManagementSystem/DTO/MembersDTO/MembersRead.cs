namespace LibraryManagementSystem.DTO.MembersDTO
{
    public class MembersRead
    {
        public int MemberId { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }
    }
}
