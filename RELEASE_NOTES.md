# Release Notes

## v1.0.0

First public release of PdfMerge.

### Highlights

- PdfMerge is available as a portable Windows desktop utility for merging PDF files locally.
- Users can add PDFs by file picker or drag and drop, reorder the selected files, and merge them into one output PDF.
- The app supports English and Hebrew, including right-to-left layout behavior for Hebrew.
- Light, dark, and system theme preferences are available from the File menu and are saved between launches.
- Portable `win-x64` packaging is available for no-install usage.

### Privacy And Storage

- PDF files stay on the local machine.
- No cloud service, account, telemetry upload, or server is required.
- Portable builds store settings and logs beside the executable under `data\`.
- Input PDFs are not modified.

### License

- Licensed under `PolyForm Noncommercial License 1.0.0`.

### Included Release Asset

- `PdfMerge-win-x64-portable.zip`

### Validation Summary

For the `v1.0.0` release baseline, the repository records:

- passing Release solution build
- passing portable publish script execution
- passing startup smoke check from the portable publish output
- manual validation tracked in `docs/manual_validation.md`

### Known Limitations

- Windows only
- current release packaging baseline is `win-x64`
- no PDF editing, splitting, compression, OCR, password removal, cloud sync, or automatic updates

### Release Tag

- `v1.0.0`
