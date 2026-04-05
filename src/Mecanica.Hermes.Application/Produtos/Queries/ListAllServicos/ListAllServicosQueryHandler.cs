using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.ListAllServicos;

internal class ListAllServicosQueryHandler(IServicoRepository repository)
    : IRequestHandler<ListAllServicosQuery, Result<PagedResult<ServicoDto>>>
{
    public async Task<Result<PagedResult<ServicoDto>>> Handle(ListAllServicosQuery request,
        CancellationToken cancellationToken)
    {
        var paginationParams = new PaginationParams { Page = request.Page, PageSize = request.PageSize };
        
        var pagedServicos = await repository.ListAllAsync(paginationParams);
        
        var servicoDtos = pagedServicos.Items.Select(s => s.ToDto()).ToList();
        
        var pagedResult = PagedResult<ServicoDto>.Create(
            servicoDtos,
            pagedServicos.Page,
            pagedServicos.PageSize,
            pagedServicos.TotalCount);

        return Result<PagedResult<ServicoDto>>.Ok(pagedResult);
    }
}
