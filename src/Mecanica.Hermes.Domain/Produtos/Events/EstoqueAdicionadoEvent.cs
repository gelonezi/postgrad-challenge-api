using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Produtos.Events;

public class EstoqueAdicionadoEvent(Guid produtoId, QuantidadeProdutoVo quantidade) : IDomainEvent
{
    public Guid ProdutoId { get; private set; } = produtoId;
    public QuantidadeProdutoVo Quantidade { get; private set; } = quantidade;
}