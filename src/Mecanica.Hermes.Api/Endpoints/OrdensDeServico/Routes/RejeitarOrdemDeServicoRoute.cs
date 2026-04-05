using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.RejeitarOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class RejeitarOrdemDeServicoRoute
{
    public static void MapRejeitarOrdemDeServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/{id:guid}/rejeitar", RejeitarOrdemDeServico)
            .WithName("RejeitarOrdemDeServico")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.AllowClienteScope)
            .WithDescription("Rejeita uma ordem de serviço");
    }

    private static async Task<IResult> RejeitarOrdemDeServico(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new RejeitarOrdemDeServicoCommand(id);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper);
    }
}
