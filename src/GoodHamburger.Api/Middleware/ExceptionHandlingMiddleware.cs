using System.Net;
using System.Text.Json;
using FluentValidation;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problem) = MapException(exception);

        if (statusCode >= HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception while processing request {Path}", context.Request.Path);
        else
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    private static (HttpStatusCode, ProblemDetails) MapException(Exception exception)
    {
        switch (exception)
        {
            case OrderNotFoundException onf:
                return (HttpStatusCode.NotFound, BuildProblem(
                    HttpStatusCode.NotFound, "Order not found", onf.Message, onf.Code));

            case InvalidMenuItemException imi:
                return (HttpStatusCode.BadRequest, BuildProblem(
                    HttpStatusCode.BadRequest, "Invalid menu item", imi.Message, imi.Code));

            case DuplicateCategoryException dup:
                return (HttpStatusCode.UnprocessableEntity, BuildProblem(
                    HttpStatusCode.UnprocessableEntity, "Duplicate category", dup.Message, dup.Code));

            case DomainException domain:
                return (HttpStatusCode.UnprocessableEntity, BuildProblem(
                    HttpStatusCode.UnprocessableEntity, "Business rule violation", domain.Message, domain.Code));

            case ValidationException validation:
            {
                var problem = BuildProblem(
                    HttpStatusCode.BadRequest, "Validation failed", "One or more validation errors occurred.", "VALIDATION_FAILED");
                problem.Extensions["errors"] = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return (HttpStatusCode.BadRequest, problem);
            }

            default:
                return (HttpStatusCode.InternalServerError, BuildProblem(
                    HttpStatusCode.InternalServerError, "Internal server error", "An unexpected error occurred.", "INTERNAL_ERROR"));
        }
    }

    private static ProblemDetails BuildProblem(HttpStatusCode status, string title, string detail, string code)
    {
        var problem = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail
        };
        problem.Extensions["code"] = code;
        return problem;
    }
}
