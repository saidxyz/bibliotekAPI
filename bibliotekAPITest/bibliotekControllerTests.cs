using bibliotekAPI.Controllers;
using Microsoft.EntityFrameworkCore;
using bibliotekAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
namespace bibliotekAPITest
{
    public class BibliotekControllerTests
    {
        
        private readonly Mock<bibliotekContext> _mockContext; // Mock the context
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
        public async Task UpdateAuthor_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var existingAuthor = new Author { FirstName = "Herman", LastName = "Melville" };
            await _authorController.CreateAuthor(existingAuthor); // Create the author

            // Update the author's first name
            existingAuthor.FirstName = "Updated Name";

            // Act
            var result = await _authorController.UpdateAuthor(existingAuthor.Id, existingAuthor);

            // Assert
            Assert.IsType<NoContentResult>(result); // Expecting NoContent
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var author = new Author { Id = 1, FirstName = "Test Name", LastName = "Test LastName" };

            // Act
            var result = await _authorController.UpdateAuthor(99, author); // Mismatched ID

            // Assert
            Assert.IsType<BadRequestResult>(result); // Expecting BadRequest
        }

        [Fact]
        public async Task UpdateAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Arrange
            var nonExistingAuthor = new Author { Id = 99, FirstName = "Nonexistent", LastName = "Author" };

            // Act
            var result = await _authorController.UpdateAuthor(99, nonExistingAuthor); // Non-existing author

            // Assert
            Assert.IsType<NotFoundResult>(result); // Expecting NotFound
        }
        

        [Theory]
        [InlineData(1)] // Example author ID
        [InlineData(99)] // Non-existent author ID
        public async Task DeleteAuthor_ReturnsExpectedResult(int authorId)
        {
            // Arrange
            var existingAuthor = new Author { Id = 1, FirstName = "Herman", LastName = "Melville" };
            await _authorController.CreateAuthor(existingAuthor); // Ensure the author is created only for valid IDs

            // Act
            var result = await _authorController.DeleteAuthor(authorId);

            // Assert
            if (authorId == 1)
            {
                Assert.IsType<NoContentResult>(result); // Expecting NoContent for existing author
            }
            else
            {
                Assert.IsType<NotFoundResult>(result); // Expecting NotFound for non-existing author
            }
        }
        
        [Fact]
        public async Task DeleteAuthor_ReturnsNotFound_WhenAuthorDoesNotExist()
        {
            // Act
            var result = await _authorController.DeleteAuthor(99); // Attempt to delete a non-existent author

            // Assert
            Assert.IsType<NotFoundResult>(result); // Expecting NotFound
        }


        // Test for GetBook by ID
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
        
        [Fact]
        public async Task GetBook_ReturnsOkResult_WithExistingBook()
        {
            // Arrange
            var existingBook = new Book
            {
                Title = "Moby Dick",
                Description = "A novel about a giant whale.",
                Year = 1851,
                AuthorId = 1
            };
            _context.Books.Add(existingBook);
            _context.SaveChanges();

            // Act
            var result = await _bookController.GetBook(existingBook.Id); // Use the generated ID

            // Assert
            var okResult = Assert.IsType<ActionResult<Book>>(result);
            var book = Assert.IsType<Book>(okResult.Value);

            Assert.Equal(existingBook.Id, book.Id); // Verify that IDs match
            Assert.Equal("Moby Dick", book.Title); // Verify title
            Assert.Equal("A novel about a giant whale.", book.Description); // Verify description
            Assert.Equal(1851, book.Year); // Verify year
            Assert.Equal(1, book.AuthorId); // Verify author ID
        }

        [Fact]
        public async Task GetBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Act
            var result = await _bookController.GetBook(99); // Using an ID that doesn't exist

            // Assert
            Assert.IsType<NotFoundResult>(result.Result); // Check that the result is NotFound
        }
        
        
        [Fact]
        public async Task CreateBook_ReturnsBadRequest_WhenBookIsNull()
        {
            // Act
            var result = await _bookController.CreateBook(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateBook_ReturnsBadRequest_WhenTitleIsMissing()
        {
            // Arrange
            var incompleteBook = new Book
            {
                Year = 2021,
                AuthorId = 1
            };

            // Act
            var result = await _bookController.CreateBook(incompleteBook);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateBook_ReturnsCreatedAtAction_WhenBookIsValid()
        {
            // Arrange
            var validBook = new Book
            {
                Title = "New Book",
                Description = "A valid description.",
                Year = 2022,
                AuthorId = 1
            };

            // Act
            var result = await _bookController.CreateBook(validBook);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdBook = Assert.IsType<Book>(createdResult.Value);

            Assert.Equal("New Book", createdBook.Title);
            Assert.Equal(2022, createdBook.Year);
        }


        [Fact]
        public async Task UpdateBook_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var existingBook = new Book
            {
                Title = "Old Title",
                Description = "Old description.",
                Year = 1900,
                AuthorId = 1
            };

            // Add the book to the context and save it to simulate an existing book in the database
            _context.Books.Add(existingBook);
            _context.SaveChanges();

            // Modify the book's properties
            var updatedBook = new Book
            {
                Id = existingBook.Id, // Keep the same ID
                Title = "Updated Title",
                Description = "Updated description.",
                Year = 1920,
                AuthorId = 1
            };

            // Act
            var result = await _bookController.UpdateBook(existingBook.Id, updatedBook);

            // Assert
            Assert.IsType<NoContentResult>(result); // Expecting NoContent on successful update

            // Verify that the book was updated in the context
            var bookInDb = await _context.Books.FindAsync(existingBook.Id);
            Assert.NotNull(bookInDb);
            Assert.Equal("Updated Title", bookInDb.Title); // Verify title was updated
            Assert.Equal("Updated description.", bookInDb.Description); // Verify description was updated
            Assert.Equal(1920, bookInDb.Year); // Verify year was updated
            Assert.Equal(1, bookInDb.AuthorId); // Verify author ID remains the same
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent_WhenBookExists()
        {
            // Arrange
            var existingBook = new Book
            {
                Title = "To Be Deleted",
                Description = "A book that will be deleted.",
                Year = 2000,
                AuthorId = 1
            };

            // Add the book to the context and save it to simulate an existing book in the database
            _context.Books.Add(existingBook);
            _context.SaveChanges();

            // Act
            var result = await _bookController.DeleteBook(existingBook.Id);

            // Assert
            Assert.IsType<NoContentResult>(result); // Expecting NoContent when deletion is successful

            // Verify that the book was removed from the context
            var bookInDb = await _context.Books.FindAsync(existingBook.Id);
            Assert.Null(bookInDb); // Book should no longer exist in the database
        }

        [Fact]
        public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
        {
            // Act
            var result = await _bookController.DeleteBook(99); // Use a non-existent book ID

            // Assert
            Assert.IsType<NotFoundResult>(result); // Expecting NotFound result for non-existent book ID
        }

        
    }   
}