using PdfMerge.Application.Merging;

namespace PdfMerge.Infrastructure.Pdf;

public sealed class DeferredPdfMergeService : IPdfMergeService
{
    public Task MergeAsync(MergeRequest request, IProgress<MergeProgress>? progress, CancellationToken cancellationToken) =>
        throw new NotSupportedException("PDF merge implementation depends on the PDF library selection task.");
}
