using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AprovarOrdemDeServico;

internal class AprovarOrdemDeServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IUnitOfWork uow,
    IOrdemDeServicoMetrics metrics)
    : IRequestHandler<AprovarOrdemDeServicoCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(AprovarOrdemDeServicoCommand request,
        CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        if (ordemDeServico.StatusAtual.StatusAtual != OrdemDeServicoStatus.AguardandoAprovacao)
        {
            metrics.ErroRegistrado("aprovar", "status_invalido");
            return Result<OrdemDeServicoDto>.BadRequest(
                "Ordem de serviço precisa estar em aguardando aprovação para ser aprovada");
        }

        var result = ordemDeServico.AvancarEtapa();
        if (result.IsFailure)
        {
            metrics.ErroRegistrado("aprovar", "domain_error");
            return Result<OrdemDeServicoDto>.BadRequest(result.Errors);
        }

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        metrics.EtapaAvancada(
            OrdemDeServicoStatus.AguardandoAprovacao.ToString(),
            ordemDeServico.StatusAtual.StatusAtual.ToString());

        var historico = ordemDeServico.HistoricosStatus[ordemDeServico.HistoricosStatus.Count - 1];
        metrics.DuracaoEtapaRegistrada(
            historico.StatusAtual.ToString(),
            (historico.DataFinalizacao!.Value - historico.DataInicio).TotalMilliseconds);

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.ToDto());
    }
}