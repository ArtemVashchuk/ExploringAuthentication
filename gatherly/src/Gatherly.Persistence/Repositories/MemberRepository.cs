using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Persistence.Repositories;

public sealed class MemberRepository(ApplicationDbContext dbContext) : IMemberRepository
{
    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext
            .Set<Member>()
            .FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

    public async Task<Member?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        await dbContext
            .Set<Member>()
            .FirstOrDefaultAsync(member => member.Email == email, cancellationToken);

    public async Task<bool> IsEmailUniqueAsync(
        Email email,
        CancellationToken cancellationToken = default) =>
        !await dbContext
            .Set<Member>()
            .AnyAsync(member => member.Email == email, cancellationToken);

    public void Add(Member member) =>
        dbContext.Set<Member>().Add(member);

    public void Update(Member member) =>
        dbContext.Set<Member>().Update(member);
}
