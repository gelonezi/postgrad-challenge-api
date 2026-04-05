using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;
using Mecanica.Hermes.Domain.OrdensDeServico.Status;
using Mecanica.Hermes.Infrastructure.Persistence.Entities;

namespace Mecanica.Hermes.Infrastructure.Persistence.Mappings;

internal static class OrdemDeServicoMapper
{
    internal static OrdemDeServico ToDomain(this OrdemDeServicoEntity entity)
    {
        var cliente = entity.Cliente.ToDomain();
        var veiculo = cliente.Veiculos.First(v => v.Id == entity.VeiculoId);

        var produtos = entity.Produtos.Select(p => OrdemDeServicoProduto.Restaurar(
            p.Id,
            p.ProdutoId,
            p.Descricao,
            p.Valor,
            p.Quantidade,
            p.Tipo,
            p.CreatedAt,
            p.UpdatedAt,
            p.IsDeleted)).ToList();

        var servicos = entity.Servicos.Select(s => OrdemDeServicoServico.Restaurar(
            s.Id,
            s.ServicoId,
            s.Descricao,
            s.Valor,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsDeleted)).ToList();

        var statusAtual = entity.StatusAtual.ToDomain();

        var historicosStatus = entity.HistoricosStatus
            .Select(h => h.ToDomain())
            .ToList();

        return OrdemDeServico.Restaurar(
            entity.Id,
            cliente,
            veiculo,
            entity.ProblemaRelatado,
            entity.Observacoes,
            statusAtual,
            historicosStatus,
            produtos,
            servicos,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.IsDeleted);
    }

    internal static OrdemDeServicoEntity ToEntity(this OrdemDeServico ordemDeServico)
    {
        var statusAtualEntity = ordemDeServico.StatusAtual.ToEntity();

        var historicosStatus = ordemDeServico.HistoricosStatus.Select(h => new OrdemDeServicoHistoricoStatusEntity(
            h.Id,
            ordemDeServico.Id,
            h.StatusAnterior,
            h.StatusAtual,
            h.StatusDestino,
            h.DataInicio,
            h.DataFinalizacao,
            h.CreatedAt,
            h.UpdatedAt,
            h.IsDeleted)).ToList();

        var produtos = ordemDeServico.Produtos.Select(p => new OrdemDeServicoProdutoEntity(
            p.Id,
            ordemDeServico.Id,
            p.ProdutoId,
            p.Descricao,
            p.Valor,
            p.Quantidade,
            p.Tipo,
            p.CreatedAt,
            p.UpdatedAt,
            p.IsDeleted)).ToList();

        var servicos = ordemDeServico.Servicos.Select(s => new OrdemDeServicoServicoEntity(
            s.Id,
            ordemDeServico.Id,
            s.ServicoId,
            s.Descricao,
            s.Valor,
            s.CreatedAt,
            s.UpdatedAt,
            s.IsDeleted)).ToList();

        return new OrdemDeServicoEntity(
            ordemDeServico.Id,
            ordemDeServico.Cliente.Id,
            ordemDeServico.Veiculo.Id,
            ordemDeServico.ProblemaRelatado,
            ordemDeServico.Observacoes,
            statusAtualEntity.Id,
            statusAtualEntity,
            historicosStatus,
            produtos,
            servicos,
            ordemDeServico.CreatedAt,
            ordemDeServico.UpdatedAt,
            ordemDeServico.IsDeleted);
    }

    private static OrdemDeServicoStatusBase ToDomain(this OrdemDeServicoStatusEntity entity)
    {
        var status = entity.StatusAtual.ToStatusBase();
        status.RestaurarBase(entity.Id, entity.CreatedAt, entity.UpdatedAt, entity.IsDeleted);
        status.StatusAnterior = entity.StatusAnterior;
        status.StatusDestino = entity.StatusDestino;
        status.DataInicio = entity.DataInicio;
        status.DataFinalizacao = entity.DataFinalizacao;
        return status;
    }

    private static OrdemDeServicoHistoricoStatus ToDomain(this OrdemDeServicoHistoricoStatusEntity entity)
    {
        return OrdemDeServicoHistoricoStatus.Restaurar(
            entity.Id,
            entity.StatusAnterior,
            entity.StatusAtual,
            entity.StatusDestino,
            entity.DataInicio,
            entity.DataFinalizacao,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.IsDeleted);
    }

    private static OrdemDeServicoStatusEntity ToEntity(this OrdemDeServicoStatusBase status)
    {
        return new OrdemDeServicoStatusEntity(
            status.Id,
            status.StatusAnterior,
            status.StatusAtual,
            status.StatusDestino,
            status.DataInicio,
            status.DataFinalizacao,
            status.CreatedAt,
            status.UpdatedAt,
            status.IsDeleted);
    }

    private static OrdemDeServicoStatusBase ToStatusBase(this OrdemDeServicoStatus status)
    {
        return status switch
        {
            OrdemDeServicoStatus.Recebida => new OrdemDeServicoRecebida(),
            OrdemDeServicoStatus.EmDiagnostico => new OrdemDeServicoEmDiagnostico(),
            OrdemDeServicoStatus.AguardandoAprovacao => new OrdemDeServicoAguardandoAprovacao(),
            OrdemDeServicoStatus.Rejeitada => new OrdemDeServicoRejeitada(),
            OrdemDeServicoStatus.EmExecucao => new OrdemDeServicoEmExecucao(),
            OrdemDeServicoStatus.Finalizada => new OrdemDeServicoFinalizada(),
            OrdemDeServicoStatus.Entregue => new OrdemDeServicoEntregue(),
            OrdemDeServicoStatus.Cancelada => new OrdemDeServicoCancelada(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
