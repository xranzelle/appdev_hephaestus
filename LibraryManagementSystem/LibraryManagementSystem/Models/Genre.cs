using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public partial class Genre
    {
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Genre name must be between 2 and 50 characters.")]
        public string GenreName { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
