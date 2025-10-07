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

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenresRead>>> GetGenres()
        {
            var genres = await _context.Genres
                .Include(g => g.Books)
                .ToListAsync();

            var genresDto = _mapper.Map<List<GenresRead>>(genres);
            return Ok(genresDto);
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenresReadByID>> GetGenre(int id)
        {
            var genre = await _context.Genres
                .Include(g => g.Books)
                .Where(g => g.GenreId == id)
                .FirstOrDefaultAsync();

            if (genre == null)
            {
                return NotFound();
            }

            var genreDto = _mapper.Map<GenresReadByID>(genre);
            return Ok(genreDto);
        }

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

        // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<GenresRead>> PostGenre(GenresPost genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);

            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();

            var genreReadDto = _mapper.Map<GenresRead>(genre);
            return CreatedAtAction("GetGenre", new { id = genre.GenreId }, genreReadDto);
        }

        // DELETE: api/Genres/5
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