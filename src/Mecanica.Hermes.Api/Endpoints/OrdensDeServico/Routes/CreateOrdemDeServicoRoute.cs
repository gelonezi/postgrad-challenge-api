using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.CreateOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class CreateOrdemDeServicoRoute
{
    public static void MapCreateOrdemDeServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/", CreateOrdemDeServico)
            .WithName("CreateOrdemDeServico")
            .Produces(statusCode: StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Cria uma nova ordem de serviço");
    }

    private static async Task<IResult> CreateOrdemDeServico(
        [FromBody] CreateOrdemDeServicoRequest request,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateOrdemDeServicoCommand>(request);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper, r =>
            Results.CreatedAtRoute(
                "GetOrdemDeServicoById",
                new { id = r.Id },
                r));
    }
}
