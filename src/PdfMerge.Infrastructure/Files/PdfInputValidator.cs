using PdfMerge.Application.Validation;
using PdfMerge.Domain.Files;
using PdfMerge.Domain.Validation;

namespace PdfMerge.Infrastructure.Files;

public sealed class PdfInputValidator : IPdfInputValidator
{
    public Task<FileValidationResult> ValidateAsync(PdfInputFile file, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!string.Equals(Path.GetExtension(file.FullPath), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(FileValidationResult.Invalid("message.invalidPdfExtension"));
        }

        if (!File.Exists(file.FullPath))
        {
            return Task.FromResult(FileValidationResult.Invalid("message.fileDoesNotExist"));
        }

        try
        {
            using var stream = File.Open(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(FileValidationResult.Valid);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return Task.FromResult(FileValidationResult.Invalid("message.fileNotReadable"));
        }
    }
}
