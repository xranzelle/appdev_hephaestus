using AutoMapper;
using LibraryManagementSystem.DTO.MembersDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public MembersController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/Members
        // Description: Returns a list of all members, ordered by MemberId.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembersRead>>> GetMembers()
        {
            var members = await _context.Members.OrderBy(m => m.MemberId).ToListAsync();

            var mappedMembers = _mapper.Map<List<MembersRead>>(members);

            return Ok(mappedMembers);
        }

        // ============================================================
        // GET: api/Members/{id}
        // Description: Returns a specific member by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<MembersReadByID>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            var mappedMember = _mapper.Map<MembersReadByID>(member);
            return Ok(mappedMember);
        }

        // ============================================================
        // PUT: api/Members/{id}
        // Description: Updates an existing member’s information.
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, MembersPut memberDto)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            _mapper.Map(memberDto, member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // POST: api/Members
        // Description: Creates a new member record.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<MembersRead>> PostMember(MembersPost memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var mappedMember = _mapper.Map<MembersRead>(member);
            return CreatedAtAction(nameof(GetMember), new { id = member.MemberId }, mappedMember);
        }

        // ============================================================
        // DELETE: api/Members/{id}
        // Description: Deletes a member record by ID.
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}