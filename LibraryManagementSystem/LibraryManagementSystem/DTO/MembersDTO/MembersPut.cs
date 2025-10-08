using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTO.MembersDTO
{
    public class MembersPut
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;

        [RegularExpression(@"^(09|\+639)\d{9}$", ErrorMessage = "Invalid PH mobile number.")]
        public string? ContactNumber { get; set; }

        [StringLength(250, ErrorMessage = "Address can't be longer than 250 characters.")]
        public string? Address { get; set; }
    }
}