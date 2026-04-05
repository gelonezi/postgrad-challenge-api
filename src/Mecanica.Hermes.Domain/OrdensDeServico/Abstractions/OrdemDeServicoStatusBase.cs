using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;

public abstract class OrdemDeServicoStatusBase : BaseDomain, IOrdemDeServicoStatus
{
    public OrdemDeServicoStatus StatusAnterior { get; set; }
    public abstract OrdemDeServicoStatus StatusAtual { get; }
    public OrdemDeServicoStatus StatusDestino { get; set; }

    protected internal abstract bool PermiteEditarProdutos { get; }
    protected abstract OrdemDeServicoStatusBase ProximoStatus { get; }
    protected abstract OrdemDeServicoStatusBase StatusCancelado { get; }

    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataFinalizacao { get; set; }

    public virtual Result<OrdemDeServicoStatusBase> AvancarEtapa(Action<IDomainEvent> addDomainEvent)
    {
        DataFinalizacao = DateTime.UtcNow;
        var statusFuturo = ProximoStatus;
        StatusDestino = statusFuturo.StatusAtual;
        return Result<OrdemDeServicoStatusBase>.Ok(statusFuturo);
    }

    public virtual Result<OrdemDeServicoStatusBase> Cancelar(Action<IDomainEvent> addDomainEvent)
    {
        DataFinalizacao = DateTime.UtcNow;
        var statusFuturo = StatusCancelado;
        StatusDestino = statusFuturo.StatusAtual;
        return Result<OrdemDeServicoStatusBase>.Ok(statusFuturo);
    }
}