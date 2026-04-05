using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AvancarEtapa;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class AvancarEtapaRoute
{
    public static void MapAvancarEtapa(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPatch("/{id:guid}/avancar", AvancarEtapa)
            .WithName("AvancarEtapa")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Avança a ordem de serviço para a próxima etapa");
    }

    private static async Task<IResult> AvancarEtapa(
        [FromRoute] Guid id,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new AvancarEtapaCommand(id);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper);
    }
}
