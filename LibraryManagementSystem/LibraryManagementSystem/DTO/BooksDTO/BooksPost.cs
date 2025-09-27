using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DTO.BooksDTO
{
    public class BooksPost
    {
        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public int GenreId { get; set; }

        public int? PublishedYear { get; set; }

        public string? Isbn { get; set; }
    }
}
