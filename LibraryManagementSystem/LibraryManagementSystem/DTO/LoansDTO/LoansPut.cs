namespace LibraryManagementSystem.DTO.LoansDTO
{
    public class LoansPut
    {
        public DateOnly DueDate { get; set; }
        public int StatusId { get; set; }
    }
}