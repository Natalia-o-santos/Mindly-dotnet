using System.Net;

namespace Mindly.Application.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message, HttpStatusCode statusCode, IDictionary<string, string[]>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public HttpStatusCode StatusCode { get; }
    public IDictionary<string, string[]>? Errors { get; }
}

