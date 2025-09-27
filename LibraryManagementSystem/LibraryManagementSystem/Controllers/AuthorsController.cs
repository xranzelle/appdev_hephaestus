using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using AutoMapper;
using LibraryManagementSystem.DTO.AuthorsDTO;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorsRead>>> GetAuthors()
        {
            var author = await _context.Authors.OrderBy(a => a.AuthorId).ToListAsync();
            var mappedAuthor = _mapper.Map<List<AuthorsRead>>(author);

            return Ok(mappedAuthor);
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorsRead>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            
            if (author == null)
            {
                return NotFound();
            }

            var mappedAuthor = _mapper.Map<AuthorsRead>(author);
            return Ok(mappedAuthor);
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorsPut authorDTO)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            var mappedAuthor = _mapper.Map(authorDTO, author);

            await _context.SaveChangesAsync();

            return Ok(mappedAuthor);
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorsRead>> PostAuthor(AuthorsPost authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.AuthorId }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
