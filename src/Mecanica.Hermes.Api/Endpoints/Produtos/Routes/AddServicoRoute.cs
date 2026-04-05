using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.Produtos.Commands.AddServico;
using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Routes;

public static class AddServicoRoute
{
    public static void MapAddServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", AddServico)
            .WithName("AddServico")
            .Produces(statusCode: StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Cria um novo serviço");
    }

    private static async Task<IResult> AddServico(
        [FromBody] AddServicoRequest request,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<AddServicoCommand>(request);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<ServicoDto, ServicoResponse>(mapper, r =>
            Results.CreatedAtRoute(
                "GetServicoById",
                new { id = r.Id },
                r));
    }
}
