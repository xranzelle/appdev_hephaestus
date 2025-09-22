namespace LibraryManagementSystem.Models;

public partial class ArchivedMember
{
    public int MemberId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public DateTime? DeletedAt { get; set; }
}