using System.Net;
using AutoMapper;
using Mecanica.Hermes.Domain.Common.Results;

namespace Mecanica.Hermes.Api.Presenter;

public static class ResultPresenterExtensions
{
    public static IResult Present<TApp, TApi>(
        this Result<TApp> result,
        IMapper mapper,
        Func<TApi, IResult>? success = null)
    {
        var apiModel = mapper.Map<TApi>(result.Data!);

        if (result.IsSuccess && success != null)
            return success(apiModel);

        if (result.IsSuccess)
            return Results.Ok(apiModel);

        return result.StatusCode switch
        {
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(),
            HttpStatusCode.Conflict => result.AsConflictResult(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.BadRequest => result.AsBadRequestResult(),
            _ => result.AsInternalServerErrorResult()
        };
    }

    public static IResult Present<TApp>(this Result<TApp> result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return result.StatusCode switch
        {
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(),
            HttpStatusCode.Conflict => result.AsConflictResult(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.BadRequest => result.AsBadRequestResult(),
            _ => result.AsInternalServerErrorResult()
        };
    }

    public static IResult Present(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        return result.StatusCode switch
        {
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.NotFound => Results.NotFound(),
            HttpStatusCode.Conflict => result.AsConflictResult(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.BadRequest => result.AsBadRequestResult(),
            _ => result.AsInternalServerErrorResult()
        };
    }

    private static IResult AsConflictResult<TApp>(this Result<TApp> result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Conflito.",
            Status = (int)HttpStatusCode.Conflict,
            Detail = "Registro já cadastrado na base da dados.",
            Errors = result.Errors
        };

        return Results.Conflict(problemDetails);
    }

    private static IResult AsConflictResult(this Result result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Conflito.",
            Status = (int)HttpStatusCode.Conflict,
            Detail = "Registro já cadastrado na base da dados.",
            Errors = result.Errors
        };

        return Results.Conflict(problemDetails);
    }

    private static IResult AsBadRequestResult<TApp>(this Result<TApp> result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Requisição Inválida.",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "A requisição contém parâmetros inválidos.",
            Errors = result.Errors
        };

        return Results.BadRequest(problemDetails);
    }

    private static IResult AsBadRequestResult(this Result result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Requisição Inválida.",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "A requisição contém parâmetros inválidos.",
            Errors = result.Errors
        };

        return Results.BadRequest(problemDetails);
    }

    private static IResult AsInternalServerErrorResult<TApp>(this Result<TApp> result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Erro Interno.",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "Ocorreu um erro inesperado no servidor.",
            Errors = result.Errors
        };

        return Results.Problem(problemDetails);
    }

    private static IResult AsInternalServerErrorResult(this Result result)
    {
        var problemDetails = new ApiProblemDetails
        {
            Title = "Erro Interno.",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "Ocorreu um erro inesperado no servidor.",
            Errors = result.Errors
        };

        return Results.Problem(problemDetails);
    }
}