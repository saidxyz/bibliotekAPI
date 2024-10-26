using System.Text.Json.Serialization;

namespace bibliotekAPI.Models
{
    public class Book
    {
        public int Id { get; set; }               // Unik identifikator for boka
        public string Title { get; set; }          // Tittel på boka
        public string? Description { get; set; }    // Beskrivelse av boka
        public int? Year { get; set; }              // Utgivelsesår
        public int? AuthorId { get; set; }          // Forfatterens ID (fremmednøkkel til Author)

        // Navigasjonsegenskap for å koble til forfatter
        [JsonIgnore]  // Ignorer denne relasjonen ved serialisering
        public Author? Author { get; set; }

    }
}