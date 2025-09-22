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

        // GET: api/Loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoansRead>>> GetLoans()
        {
            //var loans = await _context.Loans.ToListAsync();

            var loans = await _context.Loans.Include(l => l.Status).ToListAsync();

            var loansDto = _mapper.Map<List<LoansRead>>(loans);

            return Ok(loansDto);
        }

        // GET: api/Loans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoansRead>> GetLoan(int id)
        {
            var loan = await _context.Loans.Include(l => l.Status).FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null)
            {
                return NotFound();
            }

            var loans_dto = _mapper.Map<LoansRead>(loan);

            return Ok(loans_dto);
        }

        // PUT: api/Loans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, LoansPut loanDto)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            _mapper.Map(loanDto, loan);

            if (loan.StatusId == 2)
            {
                loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            }
            else
            {
                loan.ReturnDate = null;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoansRead>> PostLoan(LoansPost loanDto)
        {
            var loans = _mapper.Map<Loan>(loanDto);

            _context.Loans.Add(loans);
            await _context.SaveChangesAsync();

            var loans_dto = _mapper.Map<LoansRead>(loans);

            return CreatedAtAction("GetLoan", new { id = loans.LoanId }, loans_dto);
        }

        // DELETE: api/Loans/5
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

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanId == id);
        }
    }
}