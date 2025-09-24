namespace LibraryManagementSystem.DTO.LoanStatusDTO
{
    public class LoanStatusRead
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        public List<LoanDTO> Loans { get; set; } = new();
    }
}
