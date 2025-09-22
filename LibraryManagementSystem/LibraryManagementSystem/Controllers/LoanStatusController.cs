using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanStatusController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LoanStatusController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: api/LoanStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatus>>> GetLoanStatuses()
        {
            return await _context.LoanStatuses.ToListAsync();
        }

        // GET: api/LoanStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatus>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);

            if (loanStatus == null)
            {
                return NotFound();
            }

            return loanStatus;
        }

        // PUT: api/LoanStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoanStatus(int id, LoanStatus loanStatus)
        {
            if (id != loanStatus.StatusId)
            {
                return BadRequest();
            }

            _context.Entry(loanStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanStatusExists(id))
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

        // POST: api/LoanStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LoanStatus>> PostLoanStatus(LoanStatus loanStatus)
        {
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
