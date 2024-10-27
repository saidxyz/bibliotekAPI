using bibliotekAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using bibliotekAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace bibliotekAPITest
{
    public class BibliotekControllerTests
    {
        private readonly AuthorController _authorController;
        private readonly BookController _bookController;
        private readonly bibliotekContext _context;

        public BibliotekControllerTests()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<bibliotekContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new bibliotekContext(options);
            
            // Clear existing data to avoid conflicts
            _context.Authors.RemoveRange(_context.Authors);
            _context.Books.RemoveRange(_context.Books);
            _context.SaveChanges();

            // Seed data (optional, adjust as per your tests)
            _context.Authors.Add(new Author { FirstName = "Herman", LastName = "Melville" });
            _context.SaveChanges();
            
            // Seed data for Book
            _context.Books.Add(new Book { Id = 1, Title = "Moby Dick", Description = "A novel about a giant whale.", Year = 1851, AuthorId = 1 });
            _context.SaveChanges();


            // Instantiate controllers with context
            _authorController = new AuthorController(_context);
            _bookController = new BookController(_context);
        }

        [Fact]
        public async Task GetAuthors_ReturnsOkResult_WithListOfAuthors()
        {
            // Act
            var result = await _authorController.GetAuthors();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Author>>>(result);
            var authors = Assert.IsType<List<Author>>(okResult.Value);
    
            Assert.Single(authors);  // Check if there's only one author as seeded
            Assert.Equal("Herman", authors[0].FirstName); // Check the author's first name
            Assert.Equal("Melville", authors[0].LastName);   // Check the author's last name
            
        }
        [Fact]
        public async Task GetAuthor_ReturnsOkResult_WithExistingAuthor()
        {
            // Act
            var result = await _authorController.GetAuthor(1); // Using the seeded author ID

            // Assert
            var okResult = Assert.IsType<ActionResult<Author>>(result);
            var author = Assert.IsType<Author>(okResult.Value);
    
            Assert.Equal(1, author.Id);                 // Check if the returned author has the correct ID
            Assert.Equal("Herman", author.FirstName);   // Verify FirstName
            Assert.Equal("Melville", author.LastName);   // Verify LastName
        }

        [Fact]
        public async Task GetAuthor_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var result = await _authorController.GetAuthor(99); // Using an ID that doesn't exist

            // Assert
            Assert.IsType<NotFoundResult>(result.Result); // Check that the result is NotFound
        }
        
        [Fact]
        public async Task CreateAuthor_ReturnsCreatedAtAction_WithCreatedAuthor()
        {
            // Arrange
            var newAuthor = new Author { FirstName = "Jane", LastName = "Doe" };

            // Act
            var result = await _authorController.CreateAuthor(newAuthor);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdAuthor = Assert.IsType<Author>(createdAtActionResult.Value);
            Assert.Equal("Jane", createdAuthor.FirstName); // Check the first name
            Assert.Equal("Doe", createdAuthor.LastName); // Check the last name
            Assert.NotEqual(0, createdAuthor.Id); // Check that the Id has been set
        }

        [Fact]
        public async Task CreateAuthor_ReturnsBadRequest_WhenAuthorIsNull()
        {
            // Act
            var result = await _authorController.CreateAuthor(null); // Trying to create a null author

            // Assert
            Assert.IsType<BadRequestResult>(result.Result); // Expecting BadRequest
        }

        [Fact]
        public async Task CreateAuthor_ReturnsBadRequest_WhenAuthorIsInvalid()
        {
            // Arrange
            var invalidAuthor = new Author { FirstName = "", LastName = "Doe" }; // Invalid because FirstName is empty

            // Act
            var result = await _authorController.CreateAuthor(invalidAuthor);

            // Assert
            Assert.IsType<BadRequestResult>(result.Result); // Expecting BadRequest
        }

        [Fact]
        public async Task CreateAuthor_ReturnsConflict_WhenAuthorAlreadyExists()
        {
            // Arrange
            var existingAuthor = new Author { FirstName = "Herman", LastName = "Melville" };
            await _authorController.CreateAuthor(existingAuthor); // First, create the author

            // Act
            var result = await _authorController.CreateAuthor(existingAuthor); // Try to create the same author again

            // Assert
            Assert.IsType<ConflictResult>(result.Result); // Expecting Conflict
        }


        
        
        
        
        
        
        [Fact]
        public async Task GetBooks_ReturnsOkResult_WithListOfBooks()
        {
            // Act
            var result = await _bookController.GetBooks();

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<Book>>>(result);
            var books = Assert.IsType<List<Book>>(okResult.Value);

            Assert.Single(books); // Check if there's only one book as seeded
            Assert.Equal("Moby Dick", books[0].Title); // Check the book's title
            Assert.Equal("A novel about a giant whale.", books[0].Description); // Check the book's description
            Assert.Equal(1851, books[0].Year); // Check the book's year
        }
    }   
}