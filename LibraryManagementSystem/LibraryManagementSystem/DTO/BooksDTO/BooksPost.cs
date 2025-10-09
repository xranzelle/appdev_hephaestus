using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.BooksDTO
{
    public class BooksPost
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 100 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "AuthorId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "AuthorId must be a positive number.")]
        [DefaultValue(1)]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "GenreId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "GenreId must be a positive number.")]
        [DefaultValue(1)]
        public int GenreId { get; set; }

        [Range(1000, 2025, ErrorMessage = "PublishedYear must be between 1000 and 2025.")]
        public int? PublishedYear { get; set; }

        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be 10 or 13 digits.")]
        public string? Isbn { get; set; }
    }
}