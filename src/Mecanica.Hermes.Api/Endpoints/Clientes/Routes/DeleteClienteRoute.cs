using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.Clientes.Commands.DeleteCliente;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.Clientes.Routes;

public static class DeleteClienteRoute
{
    public static void MapDeleteCliente(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/{id:guid}", DeleteCliente)
            .WithName("DeleteCliente")
            .Produces(statusCode: StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Exclui (soft delete) um cliente pelo seu id");
    }

    private static async Task<IResult> DeleteCliente(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new DeleteClienteCommand(id), cancellationToken);
        return response.Present();
    }
}
