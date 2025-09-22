namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansRead
    {
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public int StatusId {get; set; }
    }
}