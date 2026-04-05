using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Domain.Clientes.ValueObjects;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteByEmail;

internal class GetClienteByEmailQueryHandler(IClienteRepository repository)
    : IRequestHandler<GetClienteByEmailQuery, Result<ClienteDto>>
{
    public async Task<Result<ClienteDto>> Handle(GetClienteByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = EmailVo.Criar(request.Email);
        if (email.IsFailure)
            return Result<ClienteDto>.BadRequest(email.Errors);

        var cliente = await repository.GetByEmailAsync(email.Data!);

        return cliente == null
            ? Result<ClienteDto>.NotFound()
            : Result<ClienteDto>.Ok(cliente.ToDto());
    }
}