namespace Mindly.Api.Representations;

public class PagedResource<T>
{
    public required IReadOnlyCollection<Resource<T>> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPages { get; init; }
    public required IReadOnlyCollection<LinkDto> Links { get; init; }
}

