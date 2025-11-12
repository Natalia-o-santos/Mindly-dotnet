using System.Net;

namespace Mindly.Application.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}

