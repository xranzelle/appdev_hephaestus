using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.ArchiveMembersDTO;
using AutoMapper;

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

        // GET: api/ArchivedMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivedMembersRead>>> GetArchivedMembers()
        {
            
            var member = await _context.ArchivedMembers.ToListAsync();
            var archive_memberdto = _mapper.Map<List<ArchivedMembersRead>>(member);
            
            return Ok(archive_memberdto);

        }

        // GET: api/ArchivedMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivedMembersRead>> TryLang(int id)
        {
            var archivedMember = await _context.ArchivedMembers.FindAsync(id);

            if (archivedMember == null)
            {
                return NotFound();
            }

            var archivedMemberDTO = _mapper.Map<ArchivedMembersRead>(archivedMember);

            return Ok(archivedMemberDTO);
        }

        // DELETE: api/ArchivedMembers/5
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