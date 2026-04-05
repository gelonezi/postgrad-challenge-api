using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveProduto;

internal class RemoveProdutoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IUnitOfWork uow)
    : IRequestHandler<RemoveProdutoCommand, Result>
{
    public async Task<Result> Handle(RemoveProdutoCommand request, CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result.NotFound();

        var result = ordemDeServico.RemoverProduto(request.ProdutoId);
        if (result.IsFailure)
            return Result.BadRequest(result.Errors);

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}