using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteById;

internal class GetClienteByIdQueryHandler(IClienteRepository repository)
    : IRequestHandler<GetClienteByIdQuery, Result<ClienteDto>>
{
    public async Task<Result<ClienteDto>> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.Id);

        return cliente == null
            ? Result<ClienteDto>.NotFound()
            : Result<ClienteDto>.Ok(cliente.ToDto());
    }
}