using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.SolicitarAprovacaoOrdemDeServico;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class SolicitarAprovacaoOrdemDeServicoRoute
{
    public static void MapSolicitarAprovacaoOrdemDeServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/solicitar-aprovacao", SolicitarAprovacaoOrdemDeServico)
            .WithName("SolicitarAprovacaoOrdemDeServico")
            .Produces(statusCode: StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Solicita aprovação da ordem de serviço ao cliente via e-mail");
    }

    private static async Task<IResult> SolicitarAprovacaoOrdemDeServico(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SolicitarAprovacaoOrdemDeServicoCommand(id);
        var response = await sender.Send(command, cancellationToken);

        return response.Present();
    }
}
