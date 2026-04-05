using Mecanica.Hermes.Infrastructure.Persistence.Entities.Abstractions;

namespace Mecanica.Hermes.Infrastructure.Persistence.Entities;

internal class OrdemDeServicoServicoEntity : BaseEntity
{
    private OrdemDeServicoServicoEntity()
    {
    }

    internal OrdemDeServicoServicoEntity(
        Guid id,
        Guid ordemDeServicoId,
        Guid servicoId,
        string descricao,
        decimal valor,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        Id = id;
        OrdemDeServicoId = ordemDeServicoId;
        ServicoId = servicoId;
        Descricao = descricao;
        Valor = valor;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    public Guid OrdemDeServicoId { get; private set; }
    public Guid ServicoId { get; private set; }
    public string Descricao { get; private set; } = null!;
    public decimal Valor { get; private set; }

    // Referencias EF
    public virtual OrdemDeServicoEntity OrdemDeServico { get; private set; } = null!;
}
