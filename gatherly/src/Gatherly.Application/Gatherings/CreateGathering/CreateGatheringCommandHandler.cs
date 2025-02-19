using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;
using MediatR;

namespace Gatherly.Application.Gatherings.CreateGathering;

internal sealed class CreateGatheringCommandHandler(
    IMemberRepository memberRepository,
    IGatheringRepository gatheringRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGatheringCommand>
{
    public async Task<Unit> Handle(CreateGatheringCommand request, CancellationToken cancellationToken)
    {
        Member? member = await memberRepository.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null)
        {
            return Unit.Value;
        }

        var gathering = Gathering.Create(
            Guid.NewGuid(),
            member,
            request.Type,
            request.ScheduledAtUtc,
            request.Name,
            request.Location,
            request.MaximumNumberOfAttendees,
            request.InvitationsValidBeforeInHours);

        gatheringRepository.Add(gathering);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
