using Microsoft.Extensions.Logging;
using PdfMerge.Application.Merging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerge.Infrastructure.Pdf;

public sealed class PdfSharpMergeService : IPdfMergeService
{
    private readonly ILogger<PdfSharpMergeService> _logger;

    public PdfSharpMergeService(ILogger<PdfSharpMergeService> logger)
    {
        _logger = logger;
    }

    public Task MergeAsync(MergeRequest request, IProgress<MergeProgress>? progress, CancellationToken cancellationToken) =>
        Task.Run(() => MergeCore(request, progress, cancellationToken), cancellationToken);

    private void MergeCore(MergeRequest request, IProgress<MergeProgress>? progress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            using var outputDocument = new PdfDocument();
            var totalFiles = request.InputFiles.Count;
            var completedFiles = 0;

            foreach (var inputFile in request.InputFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var inputDocument = PdfReader.Open(inputFile.FullPath, PdfDocumentOpenMode.Import);
                for (var pageIndex = 0; pageIndex < inputDocument.PageCount; pageIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    outputDocument.AddPage(inputDocument.Pages[pageIndex]);
                }

                completedFiles++;
                progress?.Report(new MergeProgress(completedFiles, totalFiles));
            }

            cancellationToken.ThrowIfCancellationRequested();
            outputDocument.Save(request.OutputPath);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PdfReaderException or InvalidOperationException)
        {
            _logger.LogError(exception, "PDF merge failed in PDFsharp adapter.");
            throw new PdfMergeException("message.mergeFailedUnsupportedPdf", exception);
        }
    }
}
