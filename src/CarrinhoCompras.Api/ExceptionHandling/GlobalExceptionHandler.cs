using CarrinhoCompras.Application.Exceptions;
using CarrinhoCompras.Domain.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CarrinhoCompras.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, titulo, detalhe) =
            exception switch
            {
                RecursoNaoEncontradoException => (
                    StatusCodes.Status404NotFound,
                    "Recurso não encontrado",
                    exception.Message),

                RegraDeNegocioException => (
                    StatusCodes.Status400BadRequest,
                    "Regra de negócio inválida",
                    exception.Message),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Erro interno do servidor",
                    "Ocorreu um erro inesperado.")
            };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro inesperado ao processar {Metodo} {Caminho}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Requisição rejeitada em {Metodo} {Caminho}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        var problema = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = detalhe,
            Instance = httpContext.Request.Path
        };

        problema.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);

        return true;
    }
}