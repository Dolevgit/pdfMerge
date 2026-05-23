# PDF Library Selection

## Selected Library

- Library: `PDFsharp`
- Package: `PDFsharp`
- Version: `6.2.4`
- Project: `src/PdfMerge.Infrastructure`
- License: MIT
- Source references:
  - https://www.nuget.org/packages/PDFsharp/
  - https://docs.pdfsharp.net/General/Overview/Choose-PDFsharp-version.html
  - https://docs.pdfsharp.net/PDFsharp/Topics/File-IO/Files-not-opened.html
  - https://docs.pdfsharp.net/PDFsharp/Topics/PDF-Features/Encryption.html

## Reasoning

`PDFsharp` is a maintained .NET PDF library with an MIT license and `net10.0` compatibility. It supports modifying, merging, and splitting existing PDF files, and it can be used from the infrastructure layer without exposing PDF library types to the UI or application projects.

The package is added only to `PdfMerge.Infrastructure`, and the app continues to depend on the application-layer `IPdfMergeService` contract.

## Current Implementation

- `PdfSharpMergeService` implements `IPdfMergeService`.
- Input files are opened with `PdfDocumentOpenMode.Import`.
- Pages are copied to a new output document in request order.
- Merge progress reports completed input file count.
- PDFsharp, file I/O, and unsupported-document failures are logged locally and wrapped in a localized application-layer merge exception.

## Validation So Far

- `dotnet build .\PdfMerge.sln` passes with zero warnings and zero errors.
- A temporary smoke harness created two ordinary one-page PDFs, merged them through `PdfSharpMergeService`, and verified that the output PDF contains two pages.
- A temporary PM-03 validation harness verified:
  - ordinary PDF merge succeeds in order
  - mixed-page-size PDF merge succeeds
  - corrupt PDFs fail with `message.mergeFailedUnsupportedPdf`
  - encrypted PDFs fail with `message.mergeFailedUnsupportedPdf`
  - unsupported file extensions fail validation with `message.invalidPdfExtension`
  - locked files fail validation with `message.fileNotReadable`

## Known Limitations

- PM-11 still owns release-level manual validation of the full Windows desktop workflow.
