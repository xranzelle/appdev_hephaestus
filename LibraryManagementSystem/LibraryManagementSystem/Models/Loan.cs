using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public partial class Loan
    {
        public int LoanId { get; set; }

        [Required(ErrorMessage = "Book ID is required.")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Member ID is required.")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Loan date is required.")]
        public DateOnly LoanDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [Required(ErrorMessage = "Due date is required.")]
        public DateOnly DueDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        [Required(ErrorMessage = "Status ID is required.")]
        public int StatusId { get; set; } = 1;

        public virtual Book Book { get; set; } = null!;
        public virtual Member Member { get; set; } = null!;
        public virtual LoanStatus Status { get; set; } = null!;
    }
}
