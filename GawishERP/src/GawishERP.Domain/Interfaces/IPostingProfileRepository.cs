using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IPostingProfileRepository
{
    Task<PostingProfile?> GetByDocumentTypeAsync(
        DocumentType documentType,
        CancellationToken cancellationToken = default);

    Task<PostingProfile?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<List<PostingProfile>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        PostingProfile postingProfile,
        CancellationToken cancellationToken = default);
}