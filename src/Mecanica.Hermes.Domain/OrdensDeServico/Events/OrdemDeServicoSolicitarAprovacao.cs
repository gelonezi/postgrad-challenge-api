using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.OrdensDeServico.Events;

public record OrdemDeServicoSolicitarAprovacao(Guid OrdemDeServicoId) : IDomainEvent;
