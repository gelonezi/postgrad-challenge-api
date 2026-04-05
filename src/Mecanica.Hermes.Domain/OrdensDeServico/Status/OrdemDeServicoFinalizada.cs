using Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico.Status;

public class OrdemDeServicoFinalizada : OrdemDeServicoStatusBase
{
    public override OrdemDeServicoStatus StatusAtual => OrdemDeServicoStatus.Finalizada;
    protected internal override bool PermiteEditarProdutos => false;
    protected override OrdemDeServicoStatusBase ProximoStatus => new OrdemDeServicoEntregue();
    protected override OrdemDeServicoStatusBase StatusCancelado => new OrdemDeServicoCancelada();
}