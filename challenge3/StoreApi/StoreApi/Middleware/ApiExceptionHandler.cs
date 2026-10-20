using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Exceptions;

namespace StoreApi.Middleware;

internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                "The request body or route values are invalid."),
            CustomerNotFoundException => (
                StatusCodes.Status404NotFound,
                "Customer not found",
                "The requested customer does not exist."),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                "The product name search term is required."),
            ProductNotFoundException => (
                StatusCodes.Status404NotFound,
                "Product not found",
                "The requested product does not exist."),
            ProductSoldOutException => (
                StatusCodes.Status409Conflict,
                "Product sold out",
                "There is not enough stock available for this product."),
            IdempotencyConflictException => (
                StatusCodes.Status409Conflict,
                "Idempotency key conflict",
                "This idempotency key was already used with a different order request."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "The server could not complete the request.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled API exception.");
        }
        else
        {
            logger.LogWarning(exception, "Invalid API request.");
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
