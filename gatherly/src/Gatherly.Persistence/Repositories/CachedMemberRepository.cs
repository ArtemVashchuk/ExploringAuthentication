using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.ValueObjects;
using Gatherly.Persistence.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Gatherly.Persistence.Repositories;

public class CachedMemberRepository(
    IMemberRepository decorated,
    IDistributedCache distributedCache,
    ApplicationDbContext dbContext)
    : IMemberRepository
{
    public async Task<Member?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string key = $"member-{id}";

        string? cachedMember = await distributedCache.GetStringAsync(
            key,
            cancellationToken);

        Member? member;
        if (string.IsNullOrEmpty(cachedMember))
        {
            member = await decorated.GetByIdAsync(id, cancellationToken);

            if (member is null)
            {
                return member;
            }

            await distributedCache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(member),
                cancellationToken);

            return member;
        }

        member = JsonConvert.DeserializeObject<Member>(
            cachedMember,
            new JsonSerializerSettings
            {
                ConstructorHandling =
                    ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });

        if (member is not null)
        {
            dbContext.Set<Member>().Attach(member);
        }

        return member;
    }

    public Task<Member?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        decorated.GetByEmailAsync(email, cancellationToken);

    public Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken = default) =>
        decorated.IsEmailUniqueAsync(email, cancellationToken);

    public void Add(Member member) => decorated.Add(member);

    public void Update(Member member) => decorated.Update(member);
}
