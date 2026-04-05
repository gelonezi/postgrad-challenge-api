using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Infrastructure.Persistence.UnitOfWork;

internal sealed class EfUnitOfWork(AppDbContext db, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    private readonly List<IDomainEvent> _pendingEvents = [];

    public void CollectEventsFrom(AggregateRoot aggregate)
    {
        _pendingEvents.AddRange(aggregate.DomainEvents);
        aggregate.ClearDomainEvents();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        var result = await db.SaveChangesAsync(cancellationToken);

        await dispatcher.DispatchAsync(_pendingEvents, cancellationToken);
        _pendingEvents.Clear();

        return result;
    }
}
