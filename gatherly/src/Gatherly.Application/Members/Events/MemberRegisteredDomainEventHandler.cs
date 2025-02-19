using Gatherly.Application.Abstractions;
using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.DomainEvents;
using Gatherly.Domain.Entities;
using Gatherly.Domain.Repositories;

namespace Gatherly.Application.Members.Events;

internal sealed class MemberRegisteredDomainEventHandler(
    IMemberRepository memberRepository,
    IEmailService emailService)
    : IDomainEventHandler<MemberRegisteredDomainEvent>
{
    public async Task Handle(
        MemberRegisteredDomainEvent notification,
        CancellationToken cancellationToken)
    {
        Member? member = await memberRepository.GetByIdAsync(
            notification.MemberId,
            cancellationToken);

        if (member is null)
        {
            return;
        }

        await emailService.SendWelcomeEmailAsync(member, cancellationToken);
    }
}
