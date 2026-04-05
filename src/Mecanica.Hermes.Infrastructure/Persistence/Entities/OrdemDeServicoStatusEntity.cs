using Mecanica.Hermes.Domain.OrdensDeServico.Enums;
using Mecanica.Hermes.Infrastructure.Persistence.Entities.Abstractions;

namespace Mecanica.Hermes.Infrastructure.Persistence.Entities;

internal class OrdemDeServicoStatusEntity : BaseEntity
{
    private OrdemDeServicoStatusEntity()
    {
    }

    internal OrdemDeServicoStatusEntity(
        Guid id,
        OrdemDeServicoStatus statusAnterior,
        OrdemDeServicoStatus statusAtual,
        OrdemDeServicoStatus statusDestino,
        DateTime dataInicio,
        DateTime? dataFinalizacao,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        Id = id;
        StatusAnterior = statusAnterior;
        StatusAtual = statusAtual;
        StatusDestino = statusDestino;
        DataInicio = dataInicio;
        DataFinalizacao = dataFinalizacao;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        IsDeleted = isDeleted;
    }

    public OrdemDeServicoStatus StatusAnterior { get; private set; }
    public OrdemDeServicoStatus StatusAtual { get; private set; }
    public OrdemDeServicoStatus StatusDestino { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
}
