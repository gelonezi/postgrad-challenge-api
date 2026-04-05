using Mecanica.Hermes.Application.Clientes.Dtos;

namespace Mecanica.Hermes.Application.OrdensDeServico.Dtos;

public sealed record OrdemDeServicoDto(
    Guid Id,
    ClienteDto Cliente,
    VeiculoDto Veiculo,
    string? ProblemaRelatado,
    string? Observacoes,
    string StatusAtual,
    decimal ValorTotal,
    List<OrdemDeServicoProdutoDto> Produtos,
    List<OrdemDeServicoServicoDto> Servicos,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IList<OrdemDeServicoHistoricoStatusDto> HistoricosStatus);
