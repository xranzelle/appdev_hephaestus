namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansByMemberIDRead
    {
        public int LoanId { get; set; }

        public int BookId { get; set; }

        public DateOnly LoanDate { get; set; }

        public DateOnly DueDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public int StatusId { get; set; } = 1;
    }
}