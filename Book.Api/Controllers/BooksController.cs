using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book.DataModel;
using Book.UI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Book.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDM>>> GetBooks()
        {
            _logger.LogInformation("Retrieving all books");
            var books = await _context.Books.OrderBy(b => b.Id).ToListAsync();
            if (books == null || !books.Any())
            {
                _logger.LogInformation("No books found");
                return Ok(new List<BookDM>()); // Return an empty list instead of an error
            }
            _logger.LogInformation($"Retrieved {books.Count} books");
            return books;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDM>> GetBook(int id)
        {
            _logger.LogInformation($"Retrieving book with id: {id}");
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                _logger.LogWarning($"Book with id {id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Retrieved book: {book.Title}");
            return book;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BookDM>> PostBook(BookDM book)
        {
            _logger.LogInformation($"Adding new book: {book.Title}");

            // Assign the smallest available ID
            book.Id = GetSmallestAvailableId();
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Added new book with id: {book.Id}");
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BookDM>> PutBook(int id, BookDM book)
        {
            _logger.LogInformation($"Attempting to update book with id: {id}");

            if (id != book.Id)
            {
                _logger.LogWarning($"ID mismatch: Route ID {id} does not match book ID {book.Id}");

                // Check if the book with the new ID already exists
                if (await _context.Books.AnyAsync(b => b.Id == book.Id))
                {
                    _logger.LogWarning($"Cannot update: A book with ID {book.Id} already exists");
                    return Conflict($"A book with ID {book.Id} already exists");
                }

                // Find the book with the old ID
                var existingBook = await _context.Books.FindAsync(id);
                if (existingBook == null)
                {
                    _logger.LogWarning($"Book with old ID {id} not found");
                    return NotFound($"Book with ID {id} not found");
                }

                // Update the information and new ID
                _context.Entry(existingBook).CurrentValues.SetValues(book);
                existingBook.Id = book.Id; // Update ID

                // Remove old entry and add new entry
                _context.Books.Remove(existingBook);
                _context.Books.Add(existingBook);
            }
            else
            {
                _context.Entry(book).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated book. New ID: {book.Id}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookExists(id))
                {
                    _logger.LogWarning($"Book with id {id} not found during update.");
                    return NotFound($"Book with ID {id} not found");
                }
                else
                {
                    _logger.LogError($"Concurrency exception when updating book: {ex.Message}");
                    return Conflict("The book was modified by another process. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating book: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the book");
            }

            return Ok(book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation($"Attempting to delete book with id: {id}");
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                _logger.LogWarning($"Book with id {id} not found");
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted book with id: {id}");
            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private int GetSmallestAvailableId()
        {
            var currentIds = _context.Books.Select(b => b.Id).ToList();
            currentIds.Sort();
            int smallestAvailableId = 1;

            foreach (var id in currentIds)
            {
                if (id == smallestAvailableId)
                {
                    smallestAvailableId++;
                }
                else
                {
                    break;
                }
            }

            return smallestAvailableId;
        }
    }
}
