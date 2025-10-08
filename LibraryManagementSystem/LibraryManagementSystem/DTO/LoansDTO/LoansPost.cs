using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPost
    {
        [Required(ErrorMessage = "MemberId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "MemberId must be a positive integer.")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "BookId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BookId must be a positive integer.")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "DueDate is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(LoansPost), nameof(ValidateDueDate))]
        public DateOnly DueDate { get; set; }

        // Custom validation method to ensure DueDate is in the future (or today)
        public static ValidationResult? ValidateDueDate(DateOnly dueDate, ValidationContext context)
        {
            if (dueDate < DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult("DueDate cannot be in the past.");
            }
            return ValidationResult.Success;
        }
    }
}
