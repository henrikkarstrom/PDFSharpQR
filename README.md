# 
# PFDSharpQR (C#)

A lightweight C# library that enables the generation of QR codes to be embedded into PDF documents using [PDFSharp](https://github.com/empira/PDFsharp) and [ZXing.Net](https://github.com/micjahn/ZXing.Net).

## Features

- Generate QR codes using ZXing.Net for encoding
- Embedds QR codes with native PDFSharp Rectangles
- Reduces the number of rectangles drawed to the PDF

## Getting Started

xGraphics.DrawQrCode("Hello World", new XRect(440, 700, 100, 100));

### Prerequisites

To use this library, you need:

- .NET 6+
- PDFSharp
- ZXing.Net

