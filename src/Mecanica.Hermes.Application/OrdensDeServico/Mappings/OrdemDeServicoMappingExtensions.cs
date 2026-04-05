using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.OrdensDeServico;

namespace Mecanica.Hermes.Application.OrdensDeServico.Mappings;

internal static class OrdemDeServicoMappingExtensions
{
    internal static OrdemDeServicoDto ToDto(this OrdemDeServico ordemDeServico)
    {
        return new OrdemDeServicoDto(
            ordemDeServico.Id,
            ordemDeServico.Cliente.ToDto(),
            ordemDeServico.Veiculo.ToDto(),
            ordemDeServico.ProblemaRelatado,
            ordemDeServico.Observacoes,
            ordemDeServico.StatusAtual.StatusAtual.ToString(),
            ordemDeServico.ValorTotal,
            ordemDeServico.Produtos.Select(p => p.ToDto()).ToList(),
            ordemDeServico.Servicos.Select(s => s.ToDto()).ToList(),
            ordemDeServico.CreatedAt,
            ordemDeServico.UpdatedAt,
            ordemDeServico.HistoricosStatus.Select(h => h.ToDto()).ToList());
    }

    internal static OrdemDeServicoProdutoDto ToDto(this OrdemDeServicoProduto produto)
    {
        return new OrdemDeServicoProdutoDto(
            produto.Id,
            produto.ProdutoId,
            produto.Descricao,
            produto.Valor,
            produto.Quantidade,
            produto.Tipo.ToString());
    }

    internal static OrdemDeServicoServicoDto ToDto(this OrdemDeServicoServico servico)
    {
        return new OrdemDeServicoServicoDto(
            servico.Id,
            servico.ServicoId,
            servico.Descricao,
            servico.Valor);
    }

    internal static OrdemDeServicoHistoricoStatusDto ToDto(this OrdemDeServicoHistoricoStatus historico)
    {
        return new OrdemDeServicoHistoricoStatusDto(
            historico.StatusAnterior,
            historico.StatusAtual,
            historico.StatusDestino,
            historico.DataInicio,
            historico.DataFinalizacao,
            historico.CreatedAt,
            historico.UpdatedAt);
    }
}
