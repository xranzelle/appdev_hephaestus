using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public partial class LoanStatus
    {
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Status name is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Status name must be between 3 and 30 characters.")]
        public string StatusName { get; set; } = string.Empty;

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
