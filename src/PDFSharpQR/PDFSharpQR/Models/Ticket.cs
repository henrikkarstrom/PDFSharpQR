using System;

namespace PDFSharpQR.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Row { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string TicketType { get; set; } = string.Empty; // General, VIP, Student, Senior, etc.
        public DateTime PurchaseDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string TicketCode { get; set; } = string.Empty; // Unique code for QR generation
        public bool IsValid { get; set; } = true;
        public bool IsUsed { get; set; } = false;
        
        // Navigation property
        public virtual Performance? Performance { get; set; }
        
        /// <summary>
        /// Generates a QR code content string containing ticket information
        /// </summary>
        public string GetQRCodeContent()
        {
            return $"TICKET:{TicketCode}|PERF:{PerformanceId}|SEAT:{Section}-{Row}-{SeatNumber}|CUSTOMER:{CustomerName}|VALID:{IsValid}";
        }
    }
}