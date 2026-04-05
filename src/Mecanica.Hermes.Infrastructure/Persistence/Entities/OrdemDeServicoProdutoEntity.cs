using Mecanica.Hermes.Domain.Produtos.Enums;
using Mecanica.Hermes.Infrastructure.Persistence.Entities.Abstractions;

namespace Mecanica.Hermes.Infrastructure.Persistence.Entities;

internal class OrdemDeServicoProdutoEntity : BaseEntity
{
    private OrdemDeServicoProdutoEntity()
    {
    }

    internal OrdemDeServicoProdutoEntity(
        Guid id,
        Guid ordemDeServicoId,
        Guid produtoId,
        string descricao,
        decimal valor,
        int quantidade,
        TipoProduto tipo,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        Id = id;
        OrdemDeServicoId = ordemDeServicoId;
        ProdutoId = produtoId;
        Descricao = descricao;
        Valor = valor;
        Quantidade = quantidade;
        Tipo = tipo;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    public Guid OrdemDeServicoId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string Descricao { get; private set; } = null!;
    public decimal Valor { get; private set; }
    public int Quantidade { get; private set; }
    public TipoProduto Tipo { get; private set; }

    // Referencias EF
    public virtual OrdemDeServicoEntity OrdemDeServico { get; private set; } = null!;
}
