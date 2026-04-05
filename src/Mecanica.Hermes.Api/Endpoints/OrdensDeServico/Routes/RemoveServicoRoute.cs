using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveServico;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class RemoveServicoRoute
{
    public static void MapRemoveServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}/servicos/{servicoId:guid}", RemoveServico)
            .WithName("RemoveServicoFromOrdemDeServico")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Remove um serviço da ordem de serviço");
    }

    private static async Task<IResult> RemoveServico(
        [FromRoute] Guid id,
        [FromRoute] Guid servicoId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveServicoCommand(id, servicoId);
        var response = await sender.Send(command, cancellationToken);

        return response.Present();
    }
}
