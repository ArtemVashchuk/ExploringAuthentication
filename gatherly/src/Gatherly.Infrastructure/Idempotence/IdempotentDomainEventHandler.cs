using Gatherly.Application.Abstractions.Messaging;
using Gatherly.Domain.Primitives;
using Gatherly.Persistence;
using Gatherly.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent>(
    INotificationHandler<TDomainEvent> decorated,
    ApplicationDbContext dbContext)
    : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        string consumer = decorated.GetType().Name;

        if (await dbContext.Set<OutboxMessageConsumer>()
                .AnyAsync(
                    outboxMessageConsumer =>
                        outboxMessageConsumer.Id == notification.Id &&
                        outboxMessageConsumer.Name == consumer,
                    cancellationToken))
        {
            return;
        }

        await decorated.Handle(notification, cancellationToken);

        dbContext.Set<OutboxMessageConsumer>()
            .Add(new OutboxMessageConsumer
            {
                Id = notification.Id,
                Name = consumer
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
