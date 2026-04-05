using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;

public interface IOrdemDeServicoStatus
{
    OrdemDeServicoStatus StatusAnterior { get; set;}
    OrdemDeServicoStatus StatusAtual { get; }
    OrdemDeServicoStatus StatusDestino { get; set;}
    DateTime DataInicio { get; set; }
    DateTime? DataFinalizacao { get; set; }
}