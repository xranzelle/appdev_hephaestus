using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.MembersDTO
{
    public class MembersPost
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid contact number format.")]
        public string? ContactNumber { get; set; }

        [StringLength(250, ErrorMessage = "Address can't be longer than 250 characters.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Membership date is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(MembersPost), nameof(ValidateMembershipDate))]
        public DateOnly MembershipDate { get; set; }

        public static ValidationResult? ValidateMembershipDate(DateOnly membershipDate, ValidationContext context)
        {
            if (membershipDate > DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult("Membership date cannot be in the future.");
            }
            return ValidationResult.Success;
        }
    }
}
