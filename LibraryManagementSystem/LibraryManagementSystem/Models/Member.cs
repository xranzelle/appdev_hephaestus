using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public partial class Member
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Member name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Contact number can't be longer than 15 characters.")]
        public string? ContactNumber { get; set; }

        [StringLength(250, ErrorMessage = "Address can't be longer than 250 characters.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Membership date is required.")]
        public DateOnly MembershipDate { get; set; }

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
