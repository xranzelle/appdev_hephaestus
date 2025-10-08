using AutoMapper;
using LibraryManagementSystem.DTO.ArchivedMembersDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivedMembersController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public ArchivedMembersController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/ArchivedMembers
        // Description: Returns a list of all archived members from the database.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivedMembersRead>>> GetArchivedMembers()
        {
            var member = await _context.ArchivedMembers.ToListAsync();

            var archive_memberdto = _mapper.Map<List<ArchivedMembersRead>>(member);

            return Ok(archive_memberdto);
        }

        // ============================================================
        // GET: api/ArchivedMembers/{id}
        // Description: Returns a specific archived member by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivedMembersReadByID>> GetArchivedMember(int id)
        {
            var archivedMember = await _context.ArchivedMembers.FindAsync(id);

            if (archivedMember == null)
            {
                return NotFound();
            }

            var archivedMemberDTO = _mapper.Map<ArchivedMembersReadByID>(archivedMember);
            return Ok(archivedMemberDTO);
        }

        // ============================================================
        // DELETE: api/ArchivedMembers/{id}
        // Description: Deletes a specific archived member from the database.
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArchivedMember(int id)
        {
            var archivedMember = await _context.ArchivedMembers.FindAsync(id);

            if (archivedMember == null)
            {
                return NotFound();
            }

            _context.ArchivedMembers.Remove(archivedMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}