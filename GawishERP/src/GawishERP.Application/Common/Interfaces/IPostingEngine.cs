using GawishERP.Application.Common.Posting;

namespace GawishERP.Application.Common.Interfaces;

public interface IPostingEngine
{
    Task<PostingResult> PostDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default);

    Task<PostingResult> ReverseDocumentAsync(
        PostingContext context,
        CancellationToken cancellationToken = default);
}