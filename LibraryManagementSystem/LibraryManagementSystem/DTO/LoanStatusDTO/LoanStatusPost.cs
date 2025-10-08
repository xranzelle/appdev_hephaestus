using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.LoanStatusDTO
{
    public class LoanStatusPost
    {
        [Required(ErrorMessage = "Status name is required.")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Status name must be between 2 and 30 characters.")]
        public string StatusName { get; set; } = string.Empty;
    }
}
