using Microsoft.EntityFrameworkCore;

namespace bibliotekAPI.Models
{
    public class bibliotekContext : DbContext
    {
        public bibliotekContext(DbContextOptions<bibliotekContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data for testing purposes
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "Herman", LastName = "Melville" },
                new Author { Id = 2, FirstName = "Jane", LastName = "Austen" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Moby Dick", Description = "A novel about a giant whale", Year = 1851, AuthorId = 1 },
                new Book { Id = 2, Title = "Pride and Prejudice", Description = "A classic romance novel", Year = 1813, AuthorId = 2 }
            );
            
            base.OnModelCreating(modelBuilder);
        }
    }
}