using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;

namespace Gatherly.Persistence.Repositories;

internal sealed class AttendeeRepository(ApplicationDbContext dbContext) : IAttendeeRepository
{
    public void Add(Attendee attendee) =>
        dbContext.Set<Attendee>().Add(attendee);
}
