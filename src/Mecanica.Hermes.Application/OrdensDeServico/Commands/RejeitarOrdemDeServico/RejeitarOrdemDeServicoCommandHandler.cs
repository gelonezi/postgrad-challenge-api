using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RejeitarOrdemDeServico;

internal class RejeitarOrdemDeServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IUnitOfWork uow,
    IOrdemDeServicoMetrics metrics)
    : IRequestHandler<RejeitarOrdemDeServicoCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(RejeitarOrdemDeServicoCommand request,
        CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        if (ordemDeServico.StatusAtual.StatusAtual != OrdemDeServicoStatus.AguardandoAprovacao)
        {
            metrics.ErroRegistrado("rejeitar", "status_invalido");
            return Result<OrdemDeServicoDto>.BadRequest(
                "Ordem de serviço precisa estar em aguardando aprovação para ser rejeitada");
        }

        var result = ordemDeServico.Cancelar();
        if (result.IsFailure)
        {
            metrics.ErroRegistrado("rejeitar", "domain_error");
            return Result<OrdemDeServicoDto>.BadRequest(result.Errors);
        }

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.ToDto());
    }
}