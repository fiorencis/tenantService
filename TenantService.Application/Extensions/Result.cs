using System.Net;

namespace TenantService.Application.Extensions;

public class Result<T>
{
    public bool IsSuccess { get; }

    public T Value { get; }

    public string ErrorMessage { get; }
    
    public HttpStatusCode StatusCode { get; }

    private Result(T value) => (IsSuccess, Value, StatusCode) = (true, value, HttpStatusCode.OK);
    private Result(string error, HttpStatusCode code) => (IsSuccess, ErrorMessage, StatusCode) = (false, error, code);

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, HttpStatusCode code) => new(error, code);
}
