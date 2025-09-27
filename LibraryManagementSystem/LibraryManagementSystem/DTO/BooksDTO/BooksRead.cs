using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DTO.BooksDTO
{
    public class BooksRead
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public virtual Author Author { get; set; } = null!;

        public virtual Genre Genre { get; set; } = null!;

        public string? Isbn { get; set; }

        public int? PublishedYear { get; set; }
    }
}
