using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPut
    {
        [Required(ErrorMessage = "DueDate is required.")]
        public DateOnly DueDate { get; set; }

        [Required(ErrorMessage = "StatusId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "StatusId must be a positive integer.")]
        public int StatusId { get; set; }
    }
}
