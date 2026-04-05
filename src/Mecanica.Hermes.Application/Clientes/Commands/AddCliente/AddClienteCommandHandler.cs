using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.AddCliente;

internal class AddClienteCommandHandler(IClienteRepository repository, IUnitOfWork uow, IClienteMetrics metrics)
    : IRequestHandler<AddClienteCommand, Result<ClienteDto>>
{
    public async Task<Result<ClienteDto>> Handle(AddClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = Cliente.Criar(
            request.NomeCivil,
            request.NomeSocial,
            request.IdentificacaoFiscal,
            request.Email,
            request.Telefone);

        if (cliente.IsFailure)
            return Result<ClienteDto>.BadRequest(cliente.Errors);

        var identificacaoFiscalJaCadastrado =
            await repository.GetByIdentificacaoFiscalAsync(cliente.Data!.IdentificacaoFiscal);
        if (identificacaoFiscalJaCadastrado is not null)
            return Result<ClienteDto>.Conflict("Identificação fiscal já cadastrada.");

        var emailJaCadastrado = await repository.GetByEmailAsync(cliente.Data!.Email);
        if (emailJaCadastrado is not null)
            return Result<ClienteDto>.Conflict("Email já cadastrado.");

        await repository.AddAsync(cliente.Data);
        await uow.CommitAsync(cancellationToken);

        metrics.ClienteCriado();

        return Result<ClienteDto>.Ok(cliente.Data.ToDto());
    }
}