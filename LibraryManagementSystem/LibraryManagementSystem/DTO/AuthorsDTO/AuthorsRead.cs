namespace LibraryManagementSystem.DTO.AuthorsDTO
{
    public class AuthorsRead
    {
        public int AuthorId { get; set; }

        public string Name { get; set; } = null!;

        public string? Nationality { get; set; }

        public DateOnly? Birthdate { get; set; }
    }
}