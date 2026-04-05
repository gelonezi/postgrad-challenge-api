using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.ListServicosByDescricao;

internal class ListServicoByDescricaoQueryHandler(IServicoRepository repository)
    : IRequestHandler<ListServicoByDescricaoQuery, Result<PagedResult<ServicoDto>>>
{
    public async Task<Result<PagedResult<ServicoDto>>> Handle(ListServicoByDescricaoQuery request, CancellationToken cancellationToken)
    {
        var descricao = DescricaoProdutoVo.Criar(request.Descricao);
        if (descricao.IsFailure)
            return Result<PagedResult<ServicoDto>>.BadRequest(descricao.Errors);

        var paginationParams = new PaginationParams { Page = request.Page, PageSize = request.PageSize };
        
        var pagedServicos = await repository.ListByDescricaoAsync(descricao.Data!, paginationParams);
        
        var servicoDtos = pagedServicos.Items.Select(s => s.ToDto()).ToList();
        
        var pagedResult = PagedResult<ServicoDto>.Create(
            servicoDtos,
            pagedServicos.Page,
            pagedServicos.PageSize,
            pagedServicos.TotalCount);

        return Result<PagedResult<ServicoDto>>.Ok(pagedResult);
    }
}