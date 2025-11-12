namespace Mindly.Api.Representations;

public class Resource<T>
{
    public required T Data { get; init; }
    public required IReadOnlyCollection<LinkDto> Links { get; init; }
}

