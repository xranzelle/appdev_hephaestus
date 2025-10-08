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
        // DELETE: api/Loans/{id}
        // Description: Deletes a loan record by ID.
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}