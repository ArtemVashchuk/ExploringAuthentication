﻿using Gatherly.Application.Abstractions;
using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.DomainEvents;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;

namespace Gatherly.Application.Invitations.Events;

internal sealed class InvitationAcceptedDomainEventHandler
    : IDomainEventHandler<InvitationAcceptedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly IGatheringRepository _gatheringRepository;

    public InvitationAcceptedDomainEventHandler(
        IEmailService emailService,
        IGatheringRepository gatheringRepository)
    {
        _emailService = emailService;
        _gatheringRepository = gatheringRepository;
    }

    public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        Gathering? gathering = await _gatheringRepository.GetByIdWithCreatorAsync(
            notification.GatheringId, cancellationToken);

        if (gathering is null)
        {
            return;
        }

        await _emailService.SendInvitationAcceptedEmailAsync(
            gathering,
            cancellationToken);
    }
}
