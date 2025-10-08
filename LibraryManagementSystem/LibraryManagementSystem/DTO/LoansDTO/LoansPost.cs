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
        public DateOnly DueDate { get; set; }
    }
}
