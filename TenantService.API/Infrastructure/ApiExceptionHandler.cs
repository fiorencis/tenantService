using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TenantService.Domain.Exceptions;

namespace TenantService.API.Infrastructure;

public sealed class ApiExceptionHandler : IExceptionHandler
{
	private readonly ILogger<ApiExceptionHandler> _logger;

	public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var problem = exception switch
		{
			UsernameAlreadyExistsException ex => new ProblemDetails
			{
				Status = StatusCodes.Status409Conflict,
				Title = "Username già presente",
				Detail = ex.Message,
				Instance = httpContext.Request.Path
			},
			ArgumentException ex => new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Richiesta non valida",
				Detail = ex.Message,
				Instance = httpContext.Request.Path
			},
			_ => new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Errore interno",
				Detail = "Si è verificato un errore inatteso.",
				Instance = httpContext.Request.Path
			}
		};

		var statusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

		if (statusCode >= StatusCodes.Status500InternalServerError)
		{
			_logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);
		}
		else
		{
			_logger.LogWarning(exception, "Handled exception while processing {Path}", httpContext.Request.Path);
		}

		httpContext.Response.StatusCode = statusCode;
		await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
		return true;
	}
}