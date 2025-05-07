using System.Diagnostics;

namespace PDFSharpQR
{
    [DebuggerDisplay("Rectangle at ({X}, {Y}) size {Width}x{Height}")]
    internal record struct Rectangle(int X, int Y, int Width, int Height);
}