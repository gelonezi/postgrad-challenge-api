using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveProduto;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class RemoveProdutoRoute
{
    public static void MapRemoveProduto(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}/produtos/{produtoId:guid}", RemoveProduto)
            .WithName("RemoveProduto")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Remove um produto da ordem de serviço");
    }

    private static async Task<IResult> RemoveProduto(
        [FromRoute] Guid id,
        [FromRoute] Guid produtoId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RemoveProdutoCommand(id, produtoId);
        var response = await sender.Send(command, cancellationToken);

        return response.Present();
    }
}
