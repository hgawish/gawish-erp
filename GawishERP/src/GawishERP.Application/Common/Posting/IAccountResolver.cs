using GawishERP.Application.Common.Posting;

namespace GawishERP.Application.Common.Posting;

public interface IAccountResolver
{
    Task<IReadOnlyCollection<AccountResolutionResult>> ResolveAsync(
        PostingContext context,
        CancellationToken cancellationToken = default);
}