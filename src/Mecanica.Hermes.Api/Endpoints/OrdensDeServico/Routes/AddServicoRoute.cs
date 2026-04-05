using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AddServico;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

public static class AddServicoRoute
{
    public static void MapAddServico(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/{id:guid}/servicos", AddServico)
            .WithName("AddServicoToOrdemDeServico")
            .Produces<OrdemDeServicoResponse>()
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Adiciona um serviço à ordem de serviço");
    }

    private static async Task<IResult> AddServico(
        [FromRoute] Guid id,
        [FromBody] AddOrdemServicoServicoRequest servicoRequest,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new AddServicoCommand(id, servicoRequest.ServicoId);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<OrdemDeServicoDto, OrdemDeServicoResponse>(mapper);
    }
}
