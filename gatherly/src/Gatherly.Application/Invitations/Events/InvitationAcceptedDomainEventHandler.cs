using Gatherly.Application.Abstractions;
using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.DomainEvents;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;

namespace Gatherly.Application.Invitations.Events;

internal sealed class InvitationAcceptedDomainEventHandler(
    IEmailService emailService,
    IGatheringRepository gatheringRepository)
    : IDomainEventHandler<InvitationAcceptedDomainEvent>
{
    public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        Gathering? gathering = await gatheringRepository.GetByIdWithCreatorAsync(
            notification.GatheringId, cancellationToken);

        if (gathering is null)
        {
            return;
        }

        await emailService.SendInvitationAcceptedEmailAsync(
            gathering,
            cancellationToken);
    }
}
