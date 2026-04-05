using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico;

public class OrdemDeServicoHistoricoStatus : BaseDomain, IOrdemDeServicoStatus
{
    private OrdemDeServicoHistoricoStatus()
    {
        // EF Constructor
    }

    internal OrdemDeServicoHistoricoStatus(IOrdemDeServicoStatus status)
    {
        StatusAnterior = status.StatusAnterior;
        StatusAtual = status.StatusAtual;
        StatusDestino = status.StatusDestino;
        DataInicio = status.DataInicio;
        DataFinalizacao = status.DataFinalizacao;
    }

    public OrdemDeServicoStatus StatusAnterior { get; set; }
    public OrdemDeServicoStatus StatusAtual { get; set; }
    public OrdemDeServicoStatus StatusDestino { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFinalizacao { get; set; }

    internal static OrdemDeServicoHistoricoStatus Restaurar(
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
        var item = new OrdemDeServicoHistoricoStatus
        {
            StatusAnterior = statusAnterior,
            StatusAtual = statusAtual,
            StatusDestino = statusDestino,
            DataInicio = dataInicio,
            DataFinalizacao = dataFinalizacao
        };
        item.RestaurarBase(id, createdAt, updatedAt, isDeleted);
        return item;
    }
}
