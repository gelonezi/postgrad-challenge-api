using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.Produtos.Events;

public sealed record ServicoExcluidoEvent(Guid ServicoId) : IDomainEvent;
