using AutoMapper;
using LibraryManagementSystem.DTO.GenresDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public GenresController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/Genres
        // Description: Returns a list of all genres.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenresRead>>> GetGenres()
        {
            var genres = await _context.Genres.OrderBy(g => g.GenreId).ToListAsync();

            var mappedGenres = _mapper.Map<List<GenresRead>>(genres);
            return Ok(mappedGenres);
        }

        // ============================================================
        // GET: api/Genres/{id}
        // Description: Returns a specific genre by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<GenresReadByID>> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            var mappedGenre = _mapper.Map<GenresReadByID>(genre);
            return Ok(mappedGenre);
        }

        // ============================================================
        // PUT: api/Genres/{id}
        // Description: Updates an existing genre’s details.
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, GenresPut genreDto)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            _mapper.Map(genreDto, genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // POST: api/Genres
        // Description: Creates a new genre record.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<GenresRead>> PostGenre(GenresPost genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var mappedGenre = _mapper.Map<GenresRead>(genre);
            return CreatedAtAction(nameof(GetGenre), new { id = genre.GenreId }, mappedGenre);
        }

        // ============================================================
        // DELETE: api/Genres/{id}
        // Description: Deletes a genre record by ID.
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}