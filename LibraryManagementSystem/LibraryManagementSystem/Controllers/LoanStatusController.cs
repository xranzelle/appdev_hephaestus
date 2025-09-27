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

        // GET: api/LoanStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatusRead>>> GetLoanStatuses()
        {
            var loans = await _context.LoanStatuses.OrderBy(l => l.StatusId).ToListAsync();
            var mappedLoans = _mapper.Map<List<LoanStatusRead>>(loans);

            return Ok(mappedLoans);
        }

        // GET: api/LoanStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatusRead>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);

            if (loanStatus == null)
            {
                return NotFound();
            }

            var mappedLoanStatus = _mapper.Map<LoanStatus>(loanStatus);
            return Ok(mappedLoanStatus);
        }

        // PUT: api/LoanStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoanStatus(int id, LoanStatusPut loanStatusDTO)
        {
            var loanStats = await _context.LoanStatuses.FindAsync(id);

            if (loanStats == null)
            {
                return NotFound();
            }

            _mapper.Map(loanStatusDTO, loanStats);
            await _context.SaveChangesAsync();

            var mappedLoanStatus = _mapper.Map<LoanStatusRead>(loanStats);

            return Ok(mappedLoanStatus);
        }

        // POST: api/LoanStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoanStatusRead>> PostLoanStatus(LoanStatusPost loanStatusDTO)
        {
            var loanStatus = _mapper.Map<LoanStatus>(loanStatusDTO);

            _context.LoanStatuses.Add(loanStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLoanStatus", new { id = loanStatus.StatusId }, loanStatus);
        }

        // DELETE: api/LoanStatus/5
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

        private bool LoanStatusExists(int id)
        {
            return _context.LoanStatuses.Any(e => e.StatusId == id);
        }
    }
}
