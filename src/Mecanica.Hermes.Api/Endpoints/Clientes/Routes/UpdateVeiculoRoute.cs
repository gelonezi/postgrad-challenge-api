using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.Clientes.Commands.UpdateVeiculo;
using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.Clientes.Routes;

public static class UpdateVeiculoRoute
{
    public static void MapUpdateVeiculo(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/veiculos/{clienteId:guid}/{veiculoId:guid}", UpdateVeiculo)
            .WithName("UpdateVeiculo")
            .Produces(statusCode: StatusCodes.Status202Accepted)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Cria um novo veículo para o cliente");
    }

    private static async Task<IResult> UpdateVeiculo(
        [FromRoute] Guid clienteId,
        [FromRoute] Guid veiculoId,
        [FromBody] UpdateVeiculoRequest request,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = new UpdateVeiculoCommand(
            clienteId,
            veiculoId,
            request.Modelo,
            request.Marca,
            request.Ano);
        var response = await sender.Send(command, cancellationToken);

        return response.Present<ClienteDto, ClienteResponse>(mapper);
    }
}