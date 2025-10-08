using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.AuthorsDTO
{
    public class AuthorsPut
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 25 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(25, ErrorMessage = "Nationality can't be longer than 25 characters.")]
        public string? Nationality { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? Birthdate { get; set; }
    }
}