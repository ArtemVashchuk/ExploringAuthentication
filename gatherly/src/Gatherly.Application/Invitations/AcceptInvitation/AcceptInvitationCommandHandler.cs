using Gatherly.Domain.Entities;
using Gatherly.Domain.Enums;
using Gatherly.Domain.Repositories;
using Gatherly.Domain.Shared;
using MediatR;

namespace Gatherly.Application.Invitations.AcceptInvitation;

internal sealed class AcceptInvitationCommandHandler(
    IGatheringRepository gatheringRepository,
    IAttendeeRepository attendeeRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AcceptInvitationCommand>
{
    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        Gathering? gathering = await gatheringRepository
            .GetByIdWithCreatorAsync(request.GatheringId, cancellationToken);

        if (gathering is null)
        {
            return Unit.Value;
        }

        Invitation? invitation = gathering.Invitations
            .FirstOrDefault(i => i.Id == request.InvitationId);

        if (invitation is null || invitation.Status != InvitationStatus.Pending)
        {
            return Unit.Value;
        }

        Result<Attendee> attendeeResult = gathering.AcceptInvitation(invitation);

        if (attendeeResult.IsSuccess)
        {
            attendeeRepository.Add(attendeeResult.Value);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}