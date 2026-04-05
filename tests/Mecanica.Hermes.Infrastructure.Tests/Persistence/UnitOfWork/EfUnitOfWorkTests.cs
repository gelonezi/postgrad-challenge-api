using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Infrastructure.Persistence;
using Mecanica.Hermes.Infrastructure.Persistence.UnitOfWork;
using Mecanica.Hermes.Infrastructure.Tests.Persistence.Helpers;

namespace Mecanica.Hermes.Infrastructure.Tests.Persistence.UnitOfWork;

public class EfUnitOfWorkTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly Mock<IDomainEventDispatcher> _dispatcherMock;
    private readonly EfUnitOfWork _unitOfWork;

    public EfUnitOfWorkTests()
    {
        _db = InMemoryDbContextFactory.Create();
        _dispatcherMock = new Mock<IDomainEventDispatcher>();
        _dispatcherMock.Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork = new EfUnitOfWork(_db, _dispatcherMock.Object);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task CommitAsync_Should_SaveChanges_And_ReturnAffectedRows()
    {
        var result = await _unitOfWork.CommitAsync(CancellationToken.None);

        result.Should().Be(0);
    }

    [Fact]
    public async Task CommitAsync_Should_DispatchCollectedDomainEvents_When_EventsWereCollected()
    {
        var cliente = Domain.Clientes.Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        _unitOfWork.CollectEventsFrom(cliente);

        IEnumerable<IDomainEvent>? capturedEvents = null;
        _dispatcherMock
            .Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<IDomainEvent>, CancellationToken>((events, _) => capturedEvents = events.ToList())
            .Returns(Task.CompletedTask);

        await _unitOfWork.CommitAsync(CancellationToken.None);

        capturedEvents.Should().NotBeNull();
        capturedEvents!.Should().NotBeEmpty();
    }

    [Fact]
    public void CollectEventsFrom_Should_ClearDomainEventsFromAggregate()
    {
        var cliente = Domain.Clientes.Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        cliente.DomainEvents.Should().HaveCount(1);

        _unitOfWork.CollectEventsFrom(cliente);

        cliente.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task CommitAsync_Should_DispatchWithEmptyList_When_NoEventsCollected()
    {
        await _unitOfWork.CommitAsync(CancellationToken.None);

        _dispatcherMock.Verify(
            d => d.DispatchAsync(
                It.Is<IEnumerable<IDomainEvent>>(events => !events.Any()),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
