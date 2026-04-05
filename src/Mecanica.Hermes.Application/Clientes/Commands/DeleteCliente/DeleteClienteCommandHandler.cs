using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.DeleteCliente;

internal class DeleteClienteCommandHandler(IClienteRepository repository, IUnitOfWork uow)
    : IRequestHandler<DeleteClienteCommand, Result>
{
    public async Task<Result> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.Id);
        if (cliente is null)
            return Result.NotFound();

        var result = cliente.Excluir();
        if (result.IsFailure)
            return Result.BadRequest(result.Errors);

        repository.Update(cliente);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
