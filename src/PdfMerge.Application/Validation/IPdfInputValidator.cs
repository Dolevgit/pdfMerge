using PdfMerge.Domain.Files;
using PdfMerge.Domain.Validation;

namespace PdfMerge.Application.Validation;

public interface IPdfInputValidator
{
    Task<FileValidationResult> ValidateAsync(PdfInputFile file, CancellationToken cancellationToken);
}
