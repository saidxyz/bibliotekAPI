﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bibliotekAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bibliotekAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly bibliotekContext _context;

        public AuthorController(bibliotekContext context)
        {
            _context = context;
        }

        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthors()
        {
            return await _context.Authors.Include(a => a.Books).ToListAsync();
        }

        // GET: api/Author/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthor(int id)
        {
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // POST: api/Author
        [HttpPost]
        public async Task<ActionResult<Author>> CreateAuthor(Author author)
        {
            if (author == null || string.IsNullOrWhiteSpace(author.FirstName) || string.IsNullOrWhiteSpace(author.LastName))
            {
                return BadRequest(); // Return BadRequest if author is null or invalid
            }

            // Check for existing authors with the same name
            if (_context.Authors.Any(a => a.FirstName == author.FirstName && a.LastName == author.LastName))
            {
                return Conflict(); // Return Conflict if the author already exists
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }



        // PUT: api/Author/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            // Check if the provided author ID matches the ID in the object
            if (id != author.Id)
            {
                return BadRequest();
            }

            // Attach the author to the context and mark it as modified
            _context.Authors.Update(author);

            try
            {
                await _context.SaveChangesAsync(); // Attempt to save changes
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Authors.Any(a => a.Id == id))
                {
                    return NotFound();
                }
                throw; // For any other concurrency exceptions, rethrow
            }

            return NoContent(); // Successfully updated
        }
        
        
        // DELETE: api/Author/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(); // Return NotFound if the author doesn't exist
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync(); // Save the changes

            return NoContent(); // Return NoContent if deletion is successful
        }




    }
}
