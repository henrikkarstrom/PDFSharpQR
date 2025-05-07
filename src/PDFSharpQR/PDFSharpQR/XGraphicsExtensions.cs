using PDFSharpQR;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ZXing;
using ZXing.QrCode.Internal;

[assembly: InternalsVisibleTo("PDFSharpQRTests")]
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace PdfSharp.Drawing
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class XGraphicsExtensions
    {
        internal static bool[,] Encode(string qrCodeContent, ErrorCorrectionLevel? errorCorrectionLevel = null, Dictionary<EncodeHintType, object>? hints = null)
        {
            if (string.IsNullOrEmpty(qrCodeContent))
            {
                throw new ArgumentNullException(nameof(qrCodeContent));
            }

            var input = Encoder.encode(qrCodeContent, errorCorrectionLevel ?? ErrorCorrectionLevel.M, hints ?? new Dictionary<EncodeHintType, object>());
            var output = new bool[input.Matrix.Width, input.Matrix.Height];

            for (int inputY = 0; inputY < input.Matrix.Height; inputY++)
            {
                // Write the contents of this row of the barcode
                for (int inputX = 0; inputX < input.Matrix.Width; inputX++)
                {
                    if (input.Matrix[inputX, inputY] == 1)
                    {
                        output[inputX, inputY] = true;
                    }
                }
            }
            return output;
        }

        internal static IEnumerable<Rectangle> Compress(bool[,] matrix)
        {
            matrix = matrix.Transpose();
            int size = matrix.GetLength(0);
            bool[,] visited = new bool[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (matrix[y, x] && !visited[y, x])
                    {
                        int xEnd = x;

                        // Expand horizontally
                        while (xEnd + 1 < size && matrix[y, xEnd + 1] && !visited[y, xEnd + 1])
                        {
                            xEnd++;
                        }

                        int yEnd = y;
                        bool expandable = true;

                        // Expand vertically to all dots on the row is black and not visited
                        while (expandable && yEnd + 1 < size)
                        {
                            for (int i = x; i <= xEnd; i++)
                            {
                                if (matrix[yEnd + 1, i] == false || visited[yEnd + 1, i])
                                {
                                    expandable = false;
                                    break;
                                }
                            }

                            if (expandable)
                            {
                                yEnd++;
                            }
                        }

                        // Mark the rectangle area as visited
                        for (int yy = y; yy <= yEnd; yy++)
                        {
                            for (int xx = x; xx <= xEnd; xx++)
                            {
                                visited[yy, xx] = true;
                            }
                        }

                        yield return new Rectangle(x, y, xEnd - x + 1, yEnd - y + 1);
                        x = xEnd; // Jump to next
                    }
                }
            }
        }

        internal static bool[,] Transpose(this bool[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            bool[,] result = new bool[cols, rows];

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    result[x, y] = matrix[y, x];
                }
            }

            return result;
        }

        
        public static void DrawQrCode(this XGraphics graphics, XRect position, string qrCodeContent, ErrorCorrectionLevel? errorCorrectionLevel = null, Dictionary<EncodeHintType, object>? hints = null, bool compressed = true)
        {
            if(graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if(position.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Width needs to be > 0");
            }

            if (position.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Height needs to be > 0");
            }

            if (string.IsNullOrEmpty(qrCodeContent))
            {
                throw new ArgumentNullException(nameof(qrCodeContent));
            }

            var bitMatrix = Encode(qrCodeContent, errorCorrectionLevel, hints);
            var pixelSize = Math.Min(position.Width, position.Height) / bitMatrix.GetLength(0);
            if (compressed)
            {
                var rectangles = Compress(bitMatrix);

                foreach (var rectangle in rectangles)
                {
                    var pixelLocation = new XPoint(position.X + (rectangle.X * pixelSize), position.Y + (rectangle.Y * pixelSize));
                    var xRectangle = new XRect(pixelLocation, new XSize(rectangle.Width * pixelSize, rectangle.Height * pixelSize));
                    graphics.DrawRectangle(XBrushes.Black, xRectangle);
                }
            }
            else
            {
                for (int x = 0; x < bitMatrix.GetLength(0); x++)
                {
                    for (int y = 0; y < bitMatrix.GetLength(1); y++)
                    {
                        if (bitMatrix[x, y])
                        {
                            var pixelLocation = new XPoint(position.X + (x * pixelSize), position.Y + (y * pixelSize));
                            var xRectangle = new XRect(pixelLocation, new XSize(pixelSize, pixelSize));
                            graphics.DrawRectangle(XBrushes.Black, xRectangle);
                        }
                    }
                }
            }
        }
    }
}
