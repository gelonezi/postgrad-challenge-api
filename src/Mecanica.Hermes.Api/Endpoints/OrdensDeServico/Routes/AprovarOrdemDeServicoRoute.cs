using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AprovarOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class AprovarOrdemDeServicoRoute
{
    public static void MapAprovarOrdemDeServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/{id:guid}/aprovar", AprovarOrdemDeServico)
            .WithName("AprovarOrdemDeServico")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.AllowClienteScope)
            .WithDescription("Aprova uma ordem de serviço");
    }

    private static async Task<IResult> AprovarOrdemDeServico(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new AprovarOrdemDeServicoCommand(id);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper);
    }
}
