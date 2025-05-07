using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;

GlobalFontSettings.FontResolver = new FailsafeFontResolver();


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
// ...and start a viewer.
PdfFileUtility.ShowDocument(filename);