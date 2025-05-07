using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PDFSharpQR;
using Xunit.Abstractions;

namespace PDFSharpQRTests
{
    public class QRGenerationTests(ITestOutputHelper testOutputHelper)
    {
        [Fact]
        public void TestCompression()
        {
            // Arrange
            var rawQrCode = XGraphicsExtensions.Encode("TEST");

            // Act
            var compressed = XGraphicsExtensions.Compress(rawQrCode);

            // Assert
            var decompressed = Decompress(compressed, rawQrCode.GetLength(0));

            AssertTrue2DArraysEqual(rawQrCode, decompressed);
        }

        private static bool[,] Decompress(IEnumerable<Rectangle> compressed, int length)
        {
            var response = new bool[length, length];
            foreach(var rectangle in compressed)
            {
                //int x = rectangle.X;
                //int y = rectangle.Y;
                for (int x = 0; x < rectangle.Width; x++)
                {
                    for(int y = 0; y < rectangle.Height; y++)
                    {
                        response[rectangle.X + x, rectangle.Y + y] = true;
                    }
                }
            }

            return response;
        }

        private static void AssertTrue2DArraysEqual(bool[,] expected, bool[,] actual)
        {
            Assert.Equal(expected.GetLength(0), actual.GetLength(0)); // Rows
            Assert.Equal(expected.GetLength(1), actual.GetLength(1)); // Columns

            for (int y = 0; y < expected.GetLength(0); y++)
            {
                for (int x = 0; x < expected.GetLength(1); x++)
                {
                    Assert.Equal(expected[y, x], actual[y, x]);
                }
            }
        }

        [Fact]
        public void CalculateCompressionTest()
        {
            // Arrange
            var reference = GenerateDocument();
            var notCompressed = GenerateDocument();
            var compressed = GenerateDocument();

            notCompressed.gaphics.DrawQrCode(new XRect(100, 100, 100, 100), "This is the reference test in the QR code", compressed: false);
            compressed.gaphics.DrawQrCode(new XRect(100, 100, 100, 100), "This is the reference test in the QR code", compressed: true);

            var referenceSize = GetDocumentSize(reference);
            var notCompressedSize = GetDocumentSize(notCompressed) - referenceSize;
            var compressedSize = GetDocumentSize(compressed) - referenceSize;

            var compression = 1d - ((double)compressedSize / (double)notCompressedSize);

            testOutputHelper.WriteLine($"Compression {compression * 100} %. {compressedSize} byte vs {notCompressedSize} byte");
        }
        
        private static long GetDocumentSize((PdfDocument document, XGraphics gaphics) reference)
        {
            using MemoryStream ms = new();
            reference.document.Save(ms, closeStream: false);
            return ms.Length;
        }

        private static (PdfDocument document, XGraphics gaphics) GenerateDocument()
        {
            var document = new PdfDocument();

            // Create an empty page in this document.
            var page = document.AddPage();
            page.Size = PageSize.A4;

            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            return (document, gfx);
        }
    }
}