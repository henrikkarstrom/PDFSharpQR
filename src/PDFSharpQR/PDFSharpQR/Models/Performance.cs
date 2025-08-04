using System;

namespace PDFSharpQR.Models
{
    public class Performance
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int VenueId { get; set; }
        public int ProducerId { get; set; }
        public string Genre { get; set; } = string.Empty; // Music, Theater, Comedy, etc.
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties (virtual for potential future EF integration)
        public virtual Venue? Venue { get; set; }
        public virtual Producer? Producer { get; set; }
    }
}