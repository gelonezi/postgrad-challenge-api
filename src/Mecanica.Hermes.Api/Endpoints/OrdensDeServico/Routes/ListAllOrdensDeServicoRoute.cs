using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.ListAllOrdensDeServico;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class ListAllOrdensDeServicoRoute
{
    public static void MapListAllOrdensDeServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", ListAllOrdensDeServico)
            .WithName("ListAllOrdensDeServico")
            .Produces<PagedResult<OrdemDeServicoResponse>>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Lista todas as ordens de serviço com paginação");
    }

    private static async Task<IResult> ListAllOrdensDeServico(
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new ListAllOrdensDeServicoQuery(page, pageSize);
        var response = await sender.Send(query, cancellationToken);

        return response.Present<PagedResult<OrdemDeServicoDto>, PagedResult<OrdemDeServicoResponse>>(mapper);
    }
}
