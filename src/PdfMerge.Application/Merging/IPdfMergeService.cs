namespace PdfMerge.Application.Merging;

public interface IPdfMergeService
{
    Task MergeAsync(MergeRequest request, IProgress<MergeProgress>? progress, CancellationToken cancellationToken);
}
