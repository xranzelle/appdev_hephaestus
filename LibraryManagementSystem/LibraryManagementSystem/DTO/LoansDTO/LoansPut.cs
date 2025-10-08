using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPut
    {
        //testing if nagana ba yung approval ni ranz
        [Required(ErrorMessage = "DueDate is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(LoansPut), nameof(ValidateDueDate))]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "StatusId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StatusId must be a positive integer.")]
        public int StatusId { get; set; }

        // Custom validation method to ensure DueDate is today or in the future
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
