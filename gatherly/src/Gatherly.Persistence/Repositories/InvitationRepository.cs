using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;

namespace Gatherly.Persistence.Repositories;

internal sealed class InvitationRepository(ApplicationDbContext dbContext) : IInvitationRepository
{
    public void Add(Invitation invitation) =>
        dbContext.Set<Invitation>().Remove(invitation);
}
