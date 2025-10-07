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

        // This method retrieves all loans from the database.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoansRead>>> GetLoans()
        {
            var loans = await _context.Loans.ToListAsync();
            var loansDto = _mapper.Map<List<LoansRead>>(loans);
            return Ok(loansDto);
        }

        // This method retrieves a specific loan by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<LoansReadByID>> GetLoan(int id)
        {
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanId == id);

            if (loan == null)
            {
                return NotFound();
            }

            var loansDto = _mapper.Map<LoansReadByID>(loan);
            return Ok(loansDto);
        }

        // This method updates an existing loan based on the provided ID and loan DTO.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoan(int id, LoansPut loanDto)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound();
            }

            // Map the changes from DTO to entity
            _mapper.Map(loanDto, loan);

            // Handle the return date based on the status
            if (loan.StatusId == 2)
            {
                loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now); // Set return date
            }
            else
            {
                loan.ReturnDate = null; // No return date if not returned
            }

            await _context.SaveChangesAsync();
            return NoContent(); // Successfully updated
        }

        // This method creates a new loan based on the provided loan DTO.
        [HttpPost]
        public async Task<ActionResult<LoansRead>> PostLoan(LoansPost loanDto)
        {
            var loan = _mapper.Map<Loan>(loanDto);

            // Add the new loan record
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            // Map the saved loan to a DTO and return the response
            var loansDto = _mapper.Map<LoansRead>(loan);
            return CreatedAtAction("GetLoan", new { id = loan.LoanId }, loansDto);
        }

        // This method deletes a loan by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            // Remove the loan from the database
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return NoContent(); // Successfully deleted
        }
    }
}