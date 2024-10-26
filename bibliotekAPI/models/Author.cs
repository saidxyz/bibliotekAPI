namespace bibliotekAPI.Models
{
    public class Author
    {
        public int Id { get; set; }               // Unik identifikator for forfatteren
        public string FirstName { get; set; }     // Fornavn
        public string LastName { get; set; }      // Etternavn

        // Navigasjonsegenskap for bøker skrevet av forfatteren
        public List<Book> Books { get; set; } = new List<Book>();
    }
}