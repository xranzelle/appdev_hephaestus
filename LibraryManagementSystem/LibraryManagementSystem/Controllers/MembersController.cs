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

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembersRead>>> GetMembers()
        {
            var members = await _context.Members.ToListAsync();
            var membersDto = _mapper.Map<List<MembersRead>>(members);
            return Ok(membersDto);
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MembersRead>> GetMember(int id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            var memberDto = _mapper.Map<MembersRead>(member);
            return Ok(memberDto);
        }

        // PUT: api/Members/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, MembersPut memberDto)
        {
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            // Map the changes from DTO to entity
            _mapper.Map(memberDto, member);

            await _context.SaveChangesAsync();

            return NoContent(); // Successfully updated
        }

        // POST: api/Members
        [HttpPost]
        public async Task<ActionResult<MembersRead>> PostMember(MembersPost memberDto)
        {
            var member = _mapper.Map<Member>(memberDto);

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            var memberReadDto = _mapper.Map<MembersRead>(member);
            return CreatedAtAction("GetMember", new { id = member.MemberId }, memberReadDto);
        }

        // DELETE: api/Members/5
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

            return NoContent(); // Successfully deleted
        }
    }
}
