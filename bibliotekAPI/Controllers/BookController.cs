﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bibliotekAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace bibliotekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly bibliotekContext _context;

        public BookController(bibliotekContext context)
        {
            _context = context;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.Include(b => b.Author).ToListAsync();
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.Include(b => b.Author).FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book data is required.");
            }
            if (string.IsNullOrEmpty(book.Title) || book.AuthorId <= 0)
            {
                return BadRequest("Title and AuthorId are required fields.");
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }



        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (id != book.Id)
            {
                return BadRequest("Book ID mismatch");
            }

            if (string.IsNullOrEmpty(book.Title) || book.AuthorId <= 0)
            {
                return BadRequest("Title and AuthorId are required fields.");
            }

            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                return NotFound("Book not found");
            }

            // Oppdater feltene her
            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Year = book.Year;
            existingBook.AuthorId = book.AuthorId;

            _context.Entry(existingBook).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



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



    }
}
