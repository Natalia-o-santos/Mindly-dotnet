using System.Net;

namespace Mindly.Application.Exceptions;

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, HttpStatusCode.Conflict)
    {
    }
}

