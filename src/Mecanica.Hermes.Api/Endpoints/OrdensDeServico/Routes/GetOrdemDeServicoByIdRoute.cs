using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.GetOrdemDeServicoById;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class GetOrdemDeServicoByIdRoute
{
    public static void MapGetOrdemDeServicoById(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{id:guid}", GetOrdemDeServicoById)
            .WithName("GetOrdemDeServicoById")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.AllowClienteScope)
            .WithDescription("Busca uma ordem de serviço por ID");
    }

    private static async Task<IResult> GetOrdemDeServicoById(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var query = new GetOrdemDeServicoByIdQuery(id);
        var response = await sender.Send(query, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper);
    }
}
