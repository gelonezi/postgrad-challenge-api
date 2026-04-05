using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveServico;

internal class RemoveServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IUnitOfWork uow)
    : IRequestHandler<RemoveServicoCommand, Result>
{
    public async Task<Result> Handle(RemoveServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result.NotFound();

        var result = ordemDeServico.RemoverServico(request.ServicoId);
        if (result.IsFailure)
            return Result.BadRequest(result.Errors);

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
