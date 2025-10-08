namespace LibraryManagementSystem.DTO.BooksDTO
{
    public class BooksRead
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public string? Isbn { get; set; }

        public int? PublishedYear { get; set; }
    }
}