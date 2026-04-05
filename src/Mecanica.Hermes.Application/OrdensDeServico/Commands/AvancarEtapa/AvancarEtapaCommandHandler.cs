using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AvancarEtapa;

internal class AvancarEtapaCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IUnitOfWork uow,
    IOrdemDeServicoMetrics metrics)
    : IRequestHandler<AvancarEtapaCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(AvancarEtapaCommand request,
        CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var statusAnterior = ordemDeServico.StatusAtual.StatusAtual.ToString();

        var result = ordemDeServico.AvancarEtapa();
        if (result.IsFailure)
        {
            metrics.ErroRegistrado("avancar_etapa", "status_invalido");
            return Result<OrdemDeServicoDto>.BadRequest(result.Errors);
        }

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        metrics.EtapaAvancada(statusAnterior, ordemDeServico.StatusAtual.StatusAtual.ToString());

        var historico = ordemDeServico.HistoricosStatus[ordemDeServico.HistoricosStatus.Count - 1];
        metrics.DuracaoEtapaRegistrada(
            historico.StatusAtual.ToString(),
            (historico.DataFinalizacao!.Value - historico.DataInicio).TotalMilliseconds);

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.ToDto());
    }
}
