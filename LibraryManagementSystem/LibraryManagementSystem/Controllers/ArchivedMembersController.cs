using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.DTO.ArchiveMembersDTO;
using AutoMapper;
using LibraryManagementSystem.DTO.AuthorsDTO;

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
            var archive_memberdto = _mapper.Map<List<AuthorsRead>>(member);
            
            return Ok(archive_memberdto);

        }

        // GET: api/ArchivedMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArchivedMembersRead>> GetArchivedMember(int id)
        {
            var archivedMember = await _context.ArchivedMembers.FindAsync(id);

            if (archivedMember == null)
            {
                return NotFound();
            }

            var archivedMemberDTO = _mapper.Map<ArchivedMembersRead>(archivedMember);

            return Ok(archivedMemberDTO);
        }

        //// PUT: api/ArchivedMembers/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutArchivedMember(int id, ArchivedMember archivedMember)
        //{
        //    if (id != archivedMember.MemberId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(archivedMember).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ArchivedMemberExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/ArchivedMembers
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<ArchivedMember>> PostArchivedMember(ArchivedMember archivedMember)
        //{
        //    _context.ArchivedMembers.Add(archivedMember);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (ArchivedMemberExists(archivedMember.MemberId))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetArchivedMember", new { id = archivedMember.MemberId }, archivedMember);
        //}

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

        private bool ArchivedMemberExists(int id)
        {
            return _context.ArchivedMembers.Any(e => e.MemberId == id);
        }
    }
}
