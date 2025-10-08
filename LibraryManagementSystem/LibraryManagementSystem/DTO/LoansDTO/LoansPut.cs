using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPut
    {
        [Required(ErrorMessage = "DueDate is required.")]
        [DataType(DataType.Date)]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "StatusId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StatusId must be a valid positive number.")]
        public int StatusId { get; set; }
    }
}