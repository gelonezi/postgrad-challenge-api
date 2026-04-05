using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.ListOrdemDeServicoByStatus;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class ListOrdemDeServicoByStatusRoute
{
    public static void MapListOrdemDeServicoByStatus(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/status/{status}", ListOrdemDeServicoByStatus)
            .WithName("ListOrdemDeServicoByStatus")
            .Produces<PagedResult<OrdemDeServicoResponse>>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Lista ordens de serviço filtradas por status com paginação");
    }

    private static async Task<IResult> ListOrdemDeServicoByStatus(
        [FromRoute] string status,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new ListOrdemDeServicoByStatusQuery(status, page, pageSize);
        var response = await sender.Send(query, cancellationToken);

        return response.Present<PagedResult<OrdemDeServicoDto>, PagedResult<OrdemDeServicoResponse>>(mapper);
    }
}
