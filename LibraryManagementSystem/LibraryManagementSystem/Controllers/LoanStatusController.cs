using AutoMapper;
using LibraryManagementSystem.DTO.LoanStatusDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanStatusController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        // Constructor to inject the context and AutoMapper service
        public LoanStatusController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // This method retrieves all loan statuses from the database, ordered by StatusId.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatusRead>>> GetLoanStatuses()
        {
            var loanStatuses = await _context.LoanStatuses.OrderBy(l => l.StatusId).ToListAsync();
            var mappedLoanStatuses = _mapper.Map<List<LoanStatusRead>>(loanStatuses);
            return Ok(mappedLoanStatuses);
        }

        // This method retrieves a specific loan status by its ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatusReadByID>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);
            if (loanStatus == null)
            {
                return NotFound();
            }
            var mappedLoanStatus = _mapper.Map<LoanStatusReadByID>(loanStatus);
            return Ok(mappedLoanStatus);
        }

        // This method updates an existing loan status based on the provided ID and DTO.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoanStatus(int id, LoanStatusPut loanStatusDTO)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);
            if (loanStatus == null)
            {
                return NotFound();
            }
            _mapper.Map(loanStatusDTO, loanStatus);
            await _context.SaveChangesAsync();
            var mappedLoanStatus = _mapper.Map<LoanStatusRead>(loanStatus);
            return Ok(mappedLoanStatus);
        }

        // This method creates a new loan status from the provided DTO.
        [HttpPost]
        public async Task<ActionResult<LoanStatusRead>> PostLoanStatus(LoanStatusPost loanStatusDTO)
        {
            var loanStatus = _mapper.Map<LoanStatus>(loanStatusDTO);
            _context.LoanStatuses.Add(loanStatus);
            await _context.SaveChangesAsync();
            var mappedLoanStatus = _mapper.Map<LoanStatusRead>(loanStatus);
            return CreatedAtAction("GetLoanStatus", new { id = loanStatus.StatusId }, mappedLoanStatus);
        }

        // This method deletes a loan status by its ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);
            if (loanStatus == null)
            {
                return NotFound();
            }
            _context.LoanStatuses.Remove(loanStatus);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}