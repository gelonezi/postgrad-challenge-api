using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico;

public class OrdemDeServicoProduto : BaseDomain
{
    internal OrdemDeServicoProduto()
    {
    }

    public Guid ProdutoId { get; private set; }
    public string Descricao { get; private set; } = null!;
    public decimal Valor { get; private set; }
    public int Quantidade { get; private set; }
    public TipoProduto Tipo { get; private set; }
    public decimal ValorTotal => Valor * Quantidade;

    internal static OrdemDeServicoProduto Criar(Produto produto, int quantidade)
    {
        return new OrdemDeServicoProduto
        {
            ProdutoId = produto.Id,
            Descricao = produto.Descricao.Valor,
            Valor = produto.Valor.Valor,
            Quantidade = quantidade,
            Tipo = produto.Tipo
        };
    }

    internal static OrdemDeServicoProduto Restaurar(
        Guid id,
        Guid produtoId,
        string descricao,
        decimal valor,
        int quantidade,
        TipoProduto tipo,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        var item = new OrdemDeServicoProduto
        {
            ProdutoId = produtoId,
            Descricao = descricao,
            Valor = valor,
            Quantidade = quantidade,
            Tipo = tipo
        };
        item.RestaurarBase(id, createdAt, updatedAt, isDeleted);
        return item;
    }
}