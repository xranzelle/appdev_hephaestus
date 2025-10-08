using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public partial class Author
    {
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [StringLength(50, ErrorMessage = "Nationality can't be longer than 50 characters.")]
        public string? Nationality { get; set; }

        // Optional: You can add a custom validation attribute or a range check if needed
        public DateOnly? Birthdate { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
