# 
# PFDSharpQR (C#)

A lightweight C# library that enables the generation of QR codes to be embedded into PDF documents using [PDFSharp](https://github.com/empira/PDFsharp) and [ZXing.Net](https://github.com/micjahn/ZXing.Net).

## Features

- Generate QR codes using ZXing.Net for encoding
- Embed QR codes using native PDFSharp rectangles
- Reduces the number of rectangles drawn to the PDF
- Fills the specified rectangle with the QR code — be sure to include a margin
- 
## Getting Started
```
xGraphics.DrawQrCode("Hello World", new XRect(440, 700, 100, 100));
```
### Prerequisites

To use this library/code, you need:

- .NET 6 or later
- PDFSharp
- ZXing.Net

