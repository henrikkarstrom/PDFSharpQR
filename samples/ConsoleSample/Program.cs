using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using PDFSharpQR.Services;

GlobalFontSettings.FontResolver = new FailsafeFontResolver();

Console.WriteLine("PDFSharpQR Demo Application");
Console.WriteLine("===========================");
Console.WriteLine("1. Basic QR Code Demo");
Console.WriteLine("2. Event Ticket Demo");
Console.WriteLine();
Console.Write("Select demo (1 or 2): ");

var choice = Console.ReadLine();

if (choice == "2")
{
    RunEventTicketDemo();
}
else
{
    RunBasicQRDemo();
}

static async void RunBasicQRDemo()
{
    Console.WriteLine("Running Basic QR Code Demo...");
    
    // Create a new PDF document.
    var document = new PdfDocument();
    document.Info.Title = "Created with PDFsharp";
    document.Info.Subject = "Just a simple QR test program.";

    // Create an empty page in this document.
    var page = document.AddPage();
    page.Size = PageSize.A4;

    // Get an XGraphics object for drawing on this page.
    var gfx = XGraphics.FromPdfPage(page);

    var headerFont = new XFont("Times New Roman", 20, XFontStyleEx.Bold);
    var font = new XFont("Times New Roman", 10, XFontStyleEx.Bold);

    // Draw the text.
    gfx.DrawString("Generated vs. Wikipedia reference QR code", headerFont, XBrushes.Black, new XRect(50, 50, 100, 0));

    // Draw generated
    gfx.DrawQrCode(new XRect(50, 100, 100, 100), "Ver1", errorCorrectionLevel: ZXing.QrCode.Internal.ErrorCorrectionLevel.H, hints: new Dictionary<ZXing.EncodeHintType, object>() { { ZXing.EncodeHintType.QR_VERSION, 1 } }, compressed: true);
    gfx.DrawString($"Generated Compressed", font, XBrushes.Black, new XRect(50, 250, 100, 0));

    gfx.DrawQrCode(new XRect(200, 100, 100, 100), "Ver1", errorCorrectionLevel: ZXing.QrCode.Internal.ErrorCorrectionLevel.H, hints: new Dictionary<ZXing.EncodeHintType, object>() { { ZXing.EncodeHintType.QR_VERSION, 1 } }, compressed: false);
    gfx.DrawString("Generated Raw", font, XBrushes.Black, new XRect(200, 250, 100, 0));

    // Draw reference
    using HttpClient httpClient = new();
    using var referenceImageStream = await httpClient.GetStreamAsync("https://upload.wikimedia.org/wikipedia/commons/5/5b/Qr-1.png");
    gfx.DrawImage(XImage.FromBitmapImageStreamThatCannotSeek(referenceImageStream), new XRect(350, 100, 100, 100));
    gfx.DrawString("Reference", font, XBrushes.Black, new XRect(350, 250, 100, 0));

    // Save the document...
    var filename = PdfFileUtility.GetTempPdfFullFileName("samples/QRCodeSample");
    document.Save(filename);
    Console.WriteLine($"Basic QR Demo PDF generated: {filename}");
    
    // ...and start a viewer.
    PdfFileUtility.ShowDocument(filename);
}

static void RunEventTicketDemo()
{
    Console.WriteLine("Running Event Ticket Demo...");
    
    // Initialize the Event Ticket Service
    var ticketService = new EventTicketService();

    // Get some sample data
    var performances = ticketService.GetPerformances().Take(3).ToList();
    var tickets = new List<PDFSharpQR.Models.Ticket>();

    foreach (var performance in performances)
    {
        var performanceTickets = ticketService.GetTicketsByPerformance(performance.Id).Take(2).ToList();
        tickets.AddRange(performanceTickets);
    }

    Console.WriteLine($"Generating PDF with QR codes for {tickets.Count} tickets across {performances.Count} performances...");

    // Create a new PDF document
    var document = new PdfDocument();
    document.Info.Title = "Event Tickets with QR Codes";
    document.Info.Subject = "Generated tickets using PDFSharpQR and EventTicketService";

    // Create an empty page
    var page = document.AddPage();
    page.Size = PageSize.A4;

    // Get an XGraphics object for drawing
    var gfx = XGraphics.FromPdfPage(page);

    // Define fonts
    var titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
    var headerFont = new XFont("Arial", 12, XFontStyleEx.Bold);
    var normalFont = new XFont("Arial", 10, XFontStyleEx.Regular);
    var smallFont = new XFont("Arial", 8, XFontStyleEx.Regular);

    // Draw title
    gfx.DrawString("Event Tickets", titleFont, XBrushes.Black, new XRect(50, 50, 500, 30), XStringFormats.Center);

    double yPosition = 100;
    int ticketNumber = 1;

    foreach (var ticket in tickets)
    {
        var performance = ticketService.GetPerformance(ticket.PerformanceId);
        var venue = ticketService.GetVenue(performance?.VenueId ?? 0);
        var producer = ticketService.GetProducer(performance?.ProducerId ?? 0);

        if (performance == null || venue == null || producer == null) continue;

        // Draw ticket border
        var ticketRect = new XRect(50, yPosition, 500, 120);
        gfx.DrawRectangle(XPens.Black, ticketRect);

        // Ticket content area
        double contentX = 60;
        double contentY = yPosition + 10;

        // Ticket header
        gfx.DrawString($"TICKET #{ticketNumber}", headerFont, XBrushes.Black, new XPoint(contentX, contentY));
        gfx.DrawString($"${ticket.Price:F2}", headerFont, XBrushes.DarkGreen, new XPoint(400, contentY));

        contentY += 20;

        // Performance details
        gfx.DrawString($"Event: {performance.Name}", normalFont, XBrushes.Black, new XPoint(contentX, contentY));
        contentY += 15;
        gfx.DrawString($"Date: {performance.Date:MMM dd, yyyy} at {performance.Date:HH:mm}", normalFont, XBrushes.Black, new XPoint(contentX, contentY));
        contentY += 15;
        gfx.DrawString($"Venue: {venue.Name}, {venue.City}, {venue.State}", normalFont, XBrushes.Black, new XPoint(contentX, contentY));
        contentY += 15;
        gfx.DrawString($"Producer: {producer.Name}", normalFont, XBrushes.Black, new XPoint(contentX, contentY));

        // Seat information
        contentY = yPosition + 70;
        gfx.DrawString($"Seat: {ticket.Section}-{ticket.Row}-{ticket.SeatNumber}", normalFont, XBrushes.Black, new XPoint(contentX, contentY));
        contentY += 15;
        gfx.DrawString($"Customer: {ticket.CustomerName}", smallFont, XBrushes.DarkBlue, new XPoint(contentX, contentY));
        contentY += 12;
        gfx.DrawString($"Ticket Type: {ticket.TicketType}", smallFont, XBrushes.DarkBlue, new XPoint(contentX, contentY));

        // Generate QR code with ticket information
        var qrCodeContent = ticket.GetQRCodeContent();
        var qrRect = new XRect(450, yPosition + 20, 80, 80);
        gfx.DrawQrCode(qrRect, qrCodeContent, compressed: true);

        // QR code label
        gfx.DrawString("Scan for entry", smallFont, XBrushes.Gray, new XRect(450, yPosition + 105, 80, 10), XStringFormats.Center);

        yPosition += 140;
        ticketNumber++;

        // Start new page if needed
        if (yPosition > 700)
        {
            page = document.AddPage();
            page.Size = PageSize.A4;
            gfx = XGraphics.FromPdfPage(page);
            yPosition = 50;
        }
    }

    // Save the document
    var filename = PdfFileUtility.GetTempPdfFullFileName("samples/EventTicketSample");
    document.Save(filename);

    Console.WriteLine($"Event Ticket Demo PDF generated: {filename}");
    Console.WriteLine();

    // Display summary information
    Console.WriteLine("Event Summary:");
    Console.WriteLine("==============");

    var allProducers = ticketService.GetProducers();
    var allVenues = ticketService.GetVenues();
    var allPerformances = ticketService.GetPerformances();
    var allTickets = ticketService.GetTickets();

    Console.WriteLine($"Total Producers: {allProducers.Count}");
    Console.WriteLine($"Total Venues: {allVenues.Count}");
    Console.WriteLine($"Total Performances: {allPerformances.Count}");
    Console.WriteLine($"Total Tickets: {allTickets.Count}");
    Console.WriteLine();

    Console.WriteLine("Sample Performances:");
    foreach (var perf in allPerformances.Take(3))
    {
        var venue = ticketService.GetVenue(perf.VenueId);
        var producer = ticketService.GetProducer(perf.ProducerId);
        var ticketCount = ticketService.GetTicketsByPerformance(perf.Id).Count;
        
        Console.WriteLine($"  - {perf.Name} on {perf.Date:MMM dd, yyyy}");
        Console.WriteLine($"    Venue: {venue?.Name} | Producer: {producer?.Name} | Tickets: {ticketCount}");
    }

    // Start the PDF viewer
    PdfFileUtility.ShowDocument(filename);
}