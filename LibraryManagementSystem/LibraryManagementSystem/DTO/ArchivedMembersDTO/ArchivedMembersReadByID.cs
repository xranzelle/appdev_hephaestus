namespace LibraryManagementSystem.DTO.ArchivedMembersDTO
{
    public class ArchivedMembersReadByID
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}