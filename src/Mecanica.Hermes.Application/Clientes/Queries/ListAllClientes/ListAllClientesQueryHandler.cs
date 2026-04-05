using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.ListAllClientes;

internal class ListAllClientesQueryHandler(IClienteRepository repository)
    : IRequestHandler<ListAllClientesQuery, Result<PagedResult<ClienteDto>>>
{
    public async Task<Result<PagedResult<ClienteDto>>> Handle(ListAllClientesQuery request,
        CancellationToken cancellationToken)
    {
        var paginationParams = new PaginationParams { Page = request.Page, PageSize = request.PageSize };
        
        var pagedClientes = await repository.ListAllAsync(paginationParams);
        
        var clienteDtos = pagedClientes.Items.Select(c => c.ToDto()).ToList();
        
        var pagedResult = PagedResult<ClienteDto>.Create(
            clienteDtos,
            pagedClientes.Page,
            pagedClientes.PageSize,
            pagedClientes.TotalCount);

        return Result<PagedResult<ClienteDto>>.Ok(pagedResult);
    }
}
