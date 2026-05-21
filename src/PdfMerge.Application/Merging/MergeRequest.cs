using PdfMerge.Domain.Files;

namespace PdfMerge.Application.Merging;

public sealed record MergeRequest(IReadOnlyList<PdfInputFile> InputFiles, string OutputPath);
