using AutoMapper;
using LibraryManagementSystem.DTO.LoansDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public LoansController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/Loans
        // Description: Returns a list of all loans.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoansRead>>> GetLoans()
        {
            var loans = await _context.Loans.OrderBy(l => l.LoanId).ToListAsync();

            var mappedLoans = _mapper.Map<List<LoansRead>>(loans);
            return Ok(mappedLoans);
        }

        // ============================================================
        // GET: api/Loans/{id}
        // Description: Returns a specific loan by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<LoansReadByID>> GetLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            var mappedLoan = _mapper.Map<LoansReadByID>(loan);
            return Ok(mappedLoan);
        }

        // ============================================================
        // PUT: api/Loans/{id}
        // Description: Updates an existing loan. Automatically sets
        // ReturnDate when StatusId == 2 (returned), otherwise null.
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, LoansPut loanDto)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            _mapper.Map(loanDto, loan);

            // Automatically handle return date logic
            loan.ReturnDate = (loan.StatusId == 2) ? DateOnly.FromDateTime(DateTime.Now) : null;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // POST: api/Loans
        // Description: Creates a new loan record.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<LoansRead>> PostLoan(LoansPost loanDto)
        {
            var loan = _mapper.Map<Loan>(loanDto);

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            var mappedLoan = _mapper.Map<LoansRead>(loan);
            return CreatedAtAction(nameof(GetLoan), new { id = loan.LoanId }, mappedLoan);
        }

        // ============================================================
        // GET: api/Loans/status/{statusId}
        // Description: Returns all loans filtered by Status ID.
        // Checks both if the Status ID exists and if any loans have that status.
        // ============================================================
        [HttpGet("status/{statusId}")]
        public async Task<ActionResult<IEnumerable<LoansByStatusIDRead>>> GetLoansByStatus(int statusId)
        {
            var statusExists = await _context.LoanStatuses.AnyAsync(s => s.StatusId == statusId);
            if (!statusExists)
            {
                return NotFound($"Status ID {statusId} not found.");
            }
                
            var loans = await _context.Loans.Where(l => l.StatusId == statusId).OrderBy(l => l.LoanDate).ToListAsync();

            if (!loans.Any())
            {
                return NotFound($"No loans found with Status ID {statusId}.");
            }
                
            var mappedLoans = _mapper.Map<List<LoansByStatusIDRead>>(loans);
            return Ok(mappedLoans);
        }

        // ============================================================
        // GET: api/Loans/member/{memberId}
        // Description: Returns all loans made by a specific member.
        // Checks if the member exists and if the member has any loans.
        // ============================================================
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<LoansByMemberIDRead>>> GetLoansByMember(int memberId)
        {
            // Check if member exists
            var memberExists = await _context.Members.AnyAsync(m => m.MemberId == memberId);
            if (!memberExists)
            {
                return NotFound($"Member with ID {memberId} not found.");
            }
                
            var loans = await _context.Loans.Where(l => l.MemberId == memberId).OrderByDescending(l => l.LoanDate).ToListAsync();

            if (!loans.Any())
            {
                return NotFound($"No loans found for Member ID {memberId}.");
            }

            var mappedLoans = _mapper.Map<List<LoansByMemberIDRead>>(loans);
            return Ok(mappedLoans);
        }
    }
}