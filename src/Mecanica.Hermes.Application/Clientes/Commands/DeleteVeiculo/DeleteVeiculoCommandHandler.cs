using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.DeleteVeiculo;

internal class DeleteVeiculoCommandHandler(IClienteRepository repository, IUnitOfWork uow)
    : IRequestHandler<DeleteVeiculoCommand, Result>
{
    public async Task<Result> Handle(DeleteVeiculoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.ClienteId);
        if (cliente is null)
            return Result.NotFound();

        var result = cliente.RemoverVeiculo(request.VeiculoId);
        if (result.IsFailure)
            return Result.BadRequest(result.Errors);

        repository.Update(cliente);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
