using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DevRoutine.Api.Middleware;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DbUpdateException { InnerException: PostgresException { SqlState: "23505" } })
        {
            return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = "Conflict",
                    Detail = "A resource with the same unique value already exists",
                    Status = StatusCodes.Status409Conflict
                }
            });
        }

        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An error occurred while processing your request. Please try again",
                Status = StatusCodes.Status500InternalServerError
            }
        });
    }
}
