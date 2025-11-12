using Mindly.Domain.Enums;

namespace Mindly.Application.DTOs.Sessions;

public class SearchFocusSessionsRequest
{
    public Guid? UserProfileId { get; set; }
    public DateTimeOffset? StartFrom { get; set; }
    public DateTimeOffset? StartTo { get; set; }
    public FocusMode? FocusMode { get; set; }
    public SessionStatus? Status { get; set; }
    public string? Keyword { get; set; }
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

