using Mecanica.Hermes.Domain.Clientes.ValueObjects;
using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.Clientes.Events;

public class ClienteCriadoEvent(IdentificacaoFiscalVo identificacaoFiscal) : IDomainEvent
{
    public IdentificacaoFiscalVo IdentificacaoFiscal { get; private set; } = identificacaoFiscal;
}