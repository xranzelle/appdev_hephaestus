namespace LibraryManagementSystem.DTO.BooksDTO
{
    public class BooksByAuthorIDRead
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public int GenreId { get; set; }

        public int? PublishedYear { get; set; }

        public string? Isbn { get; set; }
    }
}
