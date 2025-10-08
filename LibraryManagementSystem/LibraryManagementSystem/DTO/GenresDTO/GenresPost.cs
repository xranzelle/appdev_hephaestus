using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.GenresDTO
{
    public class GenresPost
    {
        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Genre name must be between 2 and 150 characters.")]
        public string GenreName { get; set; } = null!;
    }
}