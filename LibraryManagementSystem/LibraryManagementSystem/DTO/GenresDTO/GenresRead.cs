using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DTO.GenresDTO
{
    public class GenresRead
    {
        public string GenreName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
