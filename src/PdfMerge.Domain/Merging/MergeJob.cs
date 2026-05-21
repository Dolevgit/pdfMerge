using PdfMerge.Domain.Files;

namespace PdfMerge.Domain.Merging;

public sealed record MergeJob(IReadOnlyList<PdfInputFile> InputFiles, string OutputPath);
