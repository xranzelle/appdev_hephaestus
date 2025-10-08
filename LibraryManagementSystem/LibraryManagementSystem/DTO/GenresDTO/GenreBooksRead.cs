namespace LibraryManagementSystem.DTO.GenresDTO
{
    public class GenreBooksRead
    {
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public int AuthorId { get; set; }

        public int? PublishedYear { get; set; }

        public string? Isbn { get; set; }
    }
}