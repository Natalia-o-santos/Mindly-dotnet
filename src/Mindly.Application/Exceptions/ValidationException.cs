using System.Net;

namespace Mindly.Application.Exceptions;

public class ValidationException : AppException
{
    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message, HttpStatusCode.BadRequest, errors)
    {
    }
}

