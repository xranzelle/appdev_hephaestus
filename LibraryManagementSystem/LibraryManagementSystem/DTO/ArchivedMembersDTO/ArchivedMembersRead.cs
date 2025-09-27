using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.ArchiveMembersDTO
{
    public class ArchivedMembersRead
    {
        [Required]
        public string? Name { get; set; }

        public string? Email { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
