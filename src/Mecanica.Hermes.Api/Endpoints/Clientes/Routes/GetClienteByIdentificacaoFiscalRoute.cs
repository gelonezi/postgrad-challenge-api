using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;
using Mecanica.Hermes.Api.Presenter;
using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Queries.GetClienteByIdentificacaoFiscal;
using Mecanica.Hermes.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mecanica.Hermes.Api.Endpoints.Clientes.Routes;

public static class GetClienteByIdentificacaoFiscalRoute
{
    public static void MapGetClienteByIdentificacaoFiscal(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/fiscal/{identificacaoFiscal}", GetClienteByIdentificacaoFiscal)
            .WithName("GetClienteByIdentificacaoFiscal")
            .Produces<ClienteResponse>(statusCode: StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status400BadRequest)
            .Produces(statusCode: StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(statusCode: StatusCodes.Status500InternalServerError)
            .RequireAuthorization(AuthPolicies.OnlyAdminScope)
            .WithDescription("Retorna os dados do cliente pela sua identificação fiscal");
    }

    private static async Task<IResult> GetClienteByIdentificacaoFiscal(
        [FromRoute] string identificacaoFiscal,
        [FromServices] ISender sender,
        [FromServices] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(
            new GetClienteByIdentificacaoFiscalQuery(identificacaoFiscal),
            cancellationToken);
        return response.Present<ClienteDto, ClienteResponse>(mapper);
    }
}