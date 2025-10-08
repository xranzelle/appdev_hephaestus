namespace LibraryManagementSystem.DTO.AuthorsDTO
{
    public class AuthorsPost
    {
        public string Name { get; set; } = null!;

        public string? Nationality { get; set; }

        public DateOnly? Birthdate { get; set; }
    }
}