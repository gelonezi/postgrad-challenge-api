using Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico.Status;

public class OrdemDeServicoRecebida : OrdemDeServicoStatusBase
{
    public override OrdemDeServicoStatus StatusAtual => OrdemDeServicoStatus.Recebida;
    protected internal override bool PermiteEditarProdutos => true;
    protected override OrdemDeServicoStatusBase ProximoStatus => new OrdemDeServicoEmDiagnostico();
    protected override OrdemDeServicoStatusBase StatusCancelado => new OrdemDeServicoCancelada();
}