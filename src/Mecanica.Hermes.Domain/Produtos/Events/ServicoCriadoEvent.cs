using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Produtos.Events;

public class ServicoCriadoEvent(Guid id, ValorProdutoVo valor) : IDomainEvent
{
    public Guid Id { get; private set; } = id;
    public ValorProdutoVo Valor { get; private set; } = valor;
}