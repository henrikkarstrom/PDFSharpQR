namespace PDFSharpQR.Models
{
    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }
}