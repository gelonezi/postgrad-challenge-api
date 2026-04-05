using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Queries.GetServicoById;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Routes;

public static class GetServicoByIdRoute
{
    public static void MapGetServicoById(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/{id:guid}", GetServicoById)
            .WithName("GetServicoById")
            .Produces(statusCode: StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Retorna os dados do serviço pelo seu id");
    }

    internal static async Task<IResult> GetServicoById(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetServicoByIdQuery(id), cancellationToken);
        return response.Present<ServicoDto, ServicoResponse>(mapper);
    }
}
