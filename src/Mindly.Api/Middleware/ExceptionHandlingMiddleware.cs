using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Mindly.Application.Exceptions;

namespace Mindly.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        ProblemDetailsFactory problemDetailsFactory)
    {
        _next = next;
        _logger = logger;
        _problemDetailsFactory = problemDetailsFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException appException)
        {
            _logger.LogWarning(appException, "Erro de negócio capturado.");
            await WriteProblemDetailsAsync(context, appException.StatusCode, appException.Message, appException.Errors);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.LogError(dbUpdateException, "Erro de persistência.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.Conflict,
                "Não foi possível persistir os dados. Verifique se as informações já não existem ou entraram em conflito.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro inesperado.");
            await WriteProblemDetailsAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Ocorreu um erro inesperado. Tente novamente mais tarde.");
        }
    }

    private async Task WriteProblemDetailsAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        IDictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            context,
            statusCode: (int)statusCode,
            title: message,
            detail: message);

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

