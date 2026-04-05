using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Produtos.Events;

public class EstoqueIndisponivelEvent(Guid produtoId, QuantidadeProdutoVo quantidade, List<string> errors)
    : IDomainEvent
{
    public Guid ProdutoId { get; private set; } = produtoId;
    public QuantidadeProdutoVo Quantidade { get; private set; } = quantidade;
    public List<string> Errors { get; private set; } = errors;
}