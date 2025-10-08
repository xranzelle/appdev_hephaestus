using AutoMapper;
using LibraryManagementSystem.DTO.AuthorsDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // ============================================================
        // GET: api/Authors
        // Description: Returns a list of all authors, ordered by AuthorId.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorsRead>>> GetAuthors()
        {
            var authors = await _context.Authors.OrderBy(a => a.AuthorId).ToListAsync();

            var mappedAuthors = _mapper.Map<List<AuthorsRead>>(authors);

            return Ok(mappedAuthors);
        }

        // ============================================================
        // GET: api/Authors/{id}
        // Description: Returns a specific author by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorsReadByID>> GetAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            var mappedAuthor = _mapper.Map<AuthorsReadByID>(author);
            return Ok(mappedAuthor);
        }

        // ============================================================
        // PUT: api/Authors/{id}
        // Description: Updates an existing author’s details.
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorsPut authorDTO)
        {
            // Find existing author
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            // Map the updated values
            _mapper.Map(authorDTO, author);

            // Save changes
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // POST: api/Authors
        // Description: Creates a new author record in the database.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<AuthorsPost>> PostAuthor(AuthorsPost authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            // Returns 201 Created with location of the new resource
            return CreatedAtAction(nameof(GetAuthor), new { id = author.AuthorId }, author);
        }

        // ============================================================
        // DELETE: api/Authors/{id}
        // Description: Deletes an author from the database by ID.
        // ============================================================
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