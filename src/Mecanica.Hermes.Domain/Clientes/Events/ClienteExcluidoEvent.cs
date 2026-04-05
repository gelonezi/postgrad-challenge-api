using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.Clientes.Events;

public sealed record ClienteExcluidoEvent(Guid ClienteId) : IDomainEvent;
