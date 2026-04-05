using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.ListAllServicos;

public sealed record ListAllServicosQuery(
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<ServicoDto>>>;
