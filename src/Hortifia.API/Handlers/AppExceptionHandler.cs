using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Hortifia.API.Handlers;

public class AppExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetailsContext = CreateProblemDetailsContext(exception, httpContext);
        return await problemDetailsService.TryWriteAsync(problemDetailsContext);
    }

    private static ProblemDetailsContext CreateProblemDetailsContext(Exception exception, HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Status = 500,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = 500;

        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        return problemDetailsContext;
    }
}
