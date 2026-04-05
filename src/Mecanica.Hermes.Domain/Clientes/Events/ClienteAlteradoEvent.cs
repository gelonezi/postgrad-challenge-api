using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.Clientes.Events;

public class ClienteAlteradoEvent(Guid id) : IDomainEvent
{
    public Guid Id { get; private set; } = id;
}