using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;

public sealed record OrdemDeServicoResponse(
    Guid Id,
    ClienteResponse Cliente,
    VeiculoResponse Veiculo,
    string? ProblemaRelatado,
    string? Observacoes,
    string StatusAtual,
    decimal ValorTotal,
    List<OrdemDeServicoProdutoResponse> Produtos,
    List<OrdemDeServicoServicoResponse> Servicos,
    List<OrdemDeServicoHistoricoStatusResponse> HistoricosStatus,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record OrdemDeServicoProdutoResponse(
    Guid ProdutoId,
    string Descricao,
    decimal Valor,
    int Quantidade,
    string Tipo);

public sealed record OrdemDeServicoServicoResponse(
    Guid ServicoId,
    string Descricao,
    decimal Valor);

public sealed record OrdemDeServicoHistoricoStatusResponse(
    OrdemDeServicoStatus StatusAnterior,
    OrdemDeServicoStatus StatusAtual,
    OrdemDeServicoStatus StatusDestino,
    DateTime DataInicio,
    DateTime? DataFinalizacao,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
