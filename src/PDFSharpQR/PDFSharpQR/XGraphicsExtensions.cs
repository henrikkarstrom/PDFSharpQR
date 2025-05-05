using System;
using System.Collections.Generic;
using System.Drawing;
using ZXing.QrCode.Internal;

namespace PdfSharp.Drawing
{
    public static class XGraphicsExtensions
    {
        private static bool[,] Encode(string qrCodeContent, ErrorCorrectionLevel? errorCorrectionLevel)
        {
            if (string.IsNullOrEmpty(qrCodeContent))
            {
                throw new ArgumentNullException(nameof(qrCodeContent));
            }

            var input = Encoder.encode(qrCodeContent, errorCorrectionLevel ?? ErrorCorrectionLevel.M);
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

        private static IEnumerable<Rectangle> Compress(bool[,] matrix)
        {
            int size = matrix.Length;
            bool[,] visited = new bool[size, size];
            var rectangles = new List<Rectangle>();

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

        public static void DrawQrCode(this XGraphics graphics, XRect position, string qrCodeContent, ErrorCorrectionLevel? errorCorrectionLevel)
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

            var bitMatrix = Encode(qrCodeContent, errorCorrectionLevel);
            var pixelSize = Math.Min(position.Width, position.Height) / bitMatrix.Length;
            var rectangles = Compress(bitMatrix);

            foreach (var rectangle in rectangles)
            {
                var pixelLocation = new XPoint(position.X + (rectangle.X * pixelSize), position.Y + (rectangle.Y * pixelSize));
                var xRectangle = new XRect(pixelLocation, new XSize(rectangle.Width * pixelSize, rectangle.Height * pixelSize));
                graphics.DrawRectangle(XBrushes.Black, xRectangle);
            }
        }
    }
}
