using Mecanica.Hermes.Domain.Common.Abstractions;
using MediatR;

namespace Mecanica.Hermes.Infrastructure.Persistence.Events;

internal sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var e in events)
            await publisher.Publish(e, ct);
    }
}
