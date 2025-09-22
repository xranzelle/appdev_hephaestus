namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPost
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public DateOnly DueDate { get; set; }
    }
}