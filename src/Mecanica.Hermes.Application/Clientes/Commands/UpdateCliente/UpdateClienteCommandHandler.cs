using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.UpdateCliente;

internal class UpdateClienteCommandHandler(IClienteRepository repository, IUnitOfWork uow)
    : IRequestHandler<UpdateClienteCommand, Result<ClienteDto>>
{
    public async Task<Result<ClienteDto>> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.Id);

        if (cliente is null)
            return Result<ClienteDto>.NotFound();

        var result = cliente.AtualizarDados(
            request.NomeCivil,
            request.NomeSocial,
            request.Email,
            request.Telefone);

        if (result.IsFailure)
            return Result<ClienteDto>.BadRequest(result.Errors);

        repository.Update(cliente);
        await uow.CommitAsync(cancellationToken);

        return Result<ClienteDto>.Ok(cliente.ToDto());
    }
}