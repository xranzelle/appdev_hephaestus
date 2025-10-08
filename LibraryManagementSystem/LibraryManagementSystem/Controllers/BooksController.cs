using AutoMapper;
using LibraryManagementSystem.DTO.BooksDTO;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // GET: api/Books
        // Description: Returns a list of all books, ordered by BookId.
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksRead>>> GetBooks()
        {
            var books = await _context.Books
                .OrderBy(b => b.BookId)
                .ToListAsync();

            var mappedBooks = _mapper.Map<List<BooksRead>>(books);
            return Ok(mappedBooks);
        }

        // ============================================================
        // GET: api/Books/{id}
        // Description: Returns a specific book by ID.
        // ============================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<BooksReadByID>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            var mappedBook = _mapper.Map<BooksReadByID>(book);
            return Ok(mappedBook);
        }

        // ============================================================
        // PUT: api/Books/{id}
        // Description: Updates an existing book’s details.
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BooksPut bookDTO)
        {
            // Check if the book exists
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Map updated values to the existing entity
            _mapper.Map(bookDTO, book);

            // Save changes
            await _context.SaveChangesAsync();

            // Return 204 No Content (REST best practice for PUT)
            return NoContent();
        }

        // ============================================================
        // POST: api/Books
        // Description: Creates a new book record in the database.
        // ============================================================
        [HttpPost]
        public async Task<ActionResult<BooksRead>> PostBook(BooksPost bookDTO)
        {
            var book = _mapper.Map<Book>(bookDTO);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, book);
        }

        // ============================================================
        // DELETE: api/Books/{id}
        // Description: Deletes a book record by ID.
        // ============================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ============================================================
        // GET: api/Books/author/{authorId}
        // Description: Returns all books written by a specific author.
        // ============================================================
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<BooksByAuthorIDRead>>> GetBooksByAuthorId(int authorId)
        {
            // Check if author exists
            var authorExists = await _context.Authors.AnyAsync(a => a.AuthorId == authorId);
            if (!authorExists)
            {
                return NotFound($"Author with ID {authorId} not found.");
            }

            // Get books by author
            var books = await _context.Books
                .Where(b => b.AuthorId == authorId)
                .OrderBy(b => b.Title)
                .ToListAsync();

            // Map to DTO
            var mappedBooks = _mapper.Map<List<BooksByAuthorIDRead>>(books);

            if (!mappedBooks.Any())
            {
                return NotFound($"No books found for Author ID {authorId}.");
            }

            return Ok(mappedBooks);
        }
    }
}