using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.ListClientesByNome;

public sealed record ListClientesByNomeQuery(
    string Nome,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PagedResult<ClienteDto>>>;