using System.Security.AccessControl;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Persistence.Repositories;

internal sealed class GatheringRepository(ApplicationDbContext dbContext) : IGatheringRepository
{
    public async Task<List<Gathering>> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default) =>
        await ApplySpecification(new GatheringByNameSpecification(name))
            .ToListAsync(cancellationToken);

    public async Task<Gathering?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await ApplySpecification(new GatheringByIdSplitSpecification(id))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Gathering?> GetByIdWithCreatorAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await ApplySpecification(new GatheringByIdWithCreatorSpecification(id))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Gathering?> GetByIdWithInvitationsAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await dbContext.Set<Gathering>()
            .Include(g => g.Invitations)
            .Where(gathering => gathering.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    private IQueryable<Gathering> ApplySpecification(
        Specification<Gathering> specification)
    {
        return SpecificationEvaluator.GetQuery(
            dbContext.Set<Gathering>(),
            specification);
    }

    public void Add(Gathering gathering) =>
        dbContext.Set<Gathering>().Add(gathering);

    public void Remove(Gathering gathering) =>
        dbContext.Set<Gathering>().Remove(gathering);
}
