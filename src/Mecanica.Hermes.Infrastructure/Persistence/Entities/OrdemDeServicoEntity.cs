using Mecanica.Hermes.Infrastructure.Persistence.Entities.Abstractions;

namespace Mecanica.Hermes.Infrastructure.Persistence.Entities;

internal class OrdemDeServicoEntity : BaseEntity
{
    private OrdemDeServicoEntity()
    {
    }

    internal OrdemDeServicoEntity(
        Guid id,
        Guid clienteId,
        Guid veiculoId,
        string? problemaRelatado,
        string? observacoes,
        Guid statusAtualId,
        OrdemDeServicoStatusEntity statusAtual,
        ICollection<OrdemDeServicoHistoricoStatusEntity> historicosStatus,
        ICollection<OrdemDeServicoProdutoEntity> produtos,
        ICollection<OrdemDeServicoServicoEntity> servicos,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        Id = id;
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        ProblemaRelatado = problemaRelatado;
        Observacoes = observacoes;
        StatusAtualId = statusAtualId;
        StatusAtual = statusAtual;
        HistoricosStatus = historicosStatus;
        Produtos = produtos;
        Servicos = servicos;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }
    public string? ProblemaRelatado { get; private set; }
    public string? Observacoes { get; private set; }
    public Guid StatusAtualId { get; private set; }

    // Referencias EF
    public virtual ClienteEntity Cliente { get; private set; } = null!;
    public virtual VeiculoEntity Veiculo { get; private set; } = null!;
    public virtual OrdemDeServicoStatusEntity StatusAtual { get; private set; } = null!;
    public virtual ICollection<OrdemDeServicoHistoricoStatusEntity> HistoricosStatus { get; private set; } = new List<OrdemDeServicoHistoricoStatusEntity>();
    public virtual ICollection<OrdemDeServicoProdutoEntity> Produtos { get; private set; } = new List<OrdemDeServicoProdutoEntity>();
    public virtual ICollection<OrdemDeServicoServicoEntity> Servicos { get; private set; } = new List<OrdemDeServicoServicoEntity>();
}
