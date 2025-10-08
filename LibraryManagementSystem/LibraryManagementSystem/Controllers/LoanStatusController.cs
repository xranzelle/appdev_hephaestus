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

        public LoanStatusController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/LoanStatus
        // Description: Returns a list of all loan statuses, ordered by StatusId.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatusRead>>> GetLoanStatuses()
        {
            var loanStatuses = await _context.LoanStatuses.OrderBy(l => l.StatusId).ToListAsync();

            var mappedLoanStatuses = _mapper.Map<List<LoanStatusRead>>(loanStatuses);
            return Ok(mappedLoanStatuses);
        }

        // ============================================================
        // GET: api/LoanStatus/{id}
        // Description: Returns a specific loan status by ID.
        // ============================================================
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

        // ============================================================
        // PUT: api/LoanStatus/{id}
        // Description: Updates an existing loan status by ID.
        // ============================================================
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

            return NoContent();
        }

        // ============================================================
        // POST: api/LoanStatus
        // Description: Creates a new loan status record.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<LoanStatusRead>> PostLoanStatus(LoanStatusPost loanStatusDTO)
        {
            var loanStatus = _mapper.Map<LoanStatus>(loanStatusDTO);

            _context.LoanStatuses.Add(loanStatus);
            await _context.SaveChangesAsync();

            var mappedLoanStatus = _mapper.Map<LoanStatusRead>(loanStatus);
            return CreatedAtAction(nameof(GetLoanStatus), new { id = loanStatus.StatusId }, mappedLoanStatus);
        }

        // ============================================================
        // DELETE: api/LoanStatus/{id}
        // Description: Deletes a loan status record by ID.
        // ============================================================
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