using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.DTO.GenresDTO
{
    public class GenresRead
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; } = null!;
    }
}
