using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mindly.Api.Representations;
using Mindly.Application.Common;
using Mindly.Application.DTOs.Sessions;
using Mindly.Application.Services.Contracts;

namespace Mindly.Api.Controllers;

[ApiController]
[Route("api/focus-sessions")]
public class FocusSessionsController : ControllerBase
{
    private readonly IFocusSessionService _sessionService;

    public FocusSessionsController(IFocusSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public async Task<ActionResult<Resource<FocusSessionDto>>> CreateAsync(
        [FromBody] CreateFocusSessionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = session.Id }, ToResource(session));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var session = await _sessionService.GetByIdAsync(id, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _sessionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/start")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> StartAsync(
        Guid id,
        [FromBody] StartFocusSessionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionService.StartAsync(id, request, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpPut("{id:guid}/complete")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> CompleteAsync(
        Guid id,
        [FromBody] CompleteFocusSessionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionService.CompleteAsync(id, request, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> CancelAsync(
        Guid id,
        [FromBody] CancelFocusSessionRequest request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionService.CancelAsync(id, request, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpPost("{id:guid}/breaks")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> AddBreakAsync(
        Guid id,
        [FromBody] AddBreakRequest request,
        CancellationToken cancellationToken)
    {
        await _sessionService.AddBreakAsync(id, request, cancellationToken);
        var session = await _sessionService.GetByIdAsync(id, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpPost("{id:guid}/device-signals")]
    public async Task<ActionResult<Resource<FocusSessionDto>>> RecordDeviceSignalAsync(
        Guid id,
        [FromBody] RecordDeviceSignalRequest request,
        CancellationToken cancellationToken)
    {
        await _sessionService.RecordDeviceSignalAsync(id, request, cancellationToken);
        var session = await _sessionService.GetByIdAsync(id, cancellationToken);
        return Ok(ToResource(session));
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResource<FocusSessionDto>>> SearchAsync(
        [FromQuery] SearchFocusSessionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sessionService.SearchAsync(request, cancellationToken);

        var pagedResource = new PagedResource<FocusSessionDto>
        {
            Items = result.Items.Select(ToResource).ToArray(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            Links = BuildPaginationLinks(request, result)
        };

        return Ok(pagedResource);
    }

    private Resource<FocusSessionDto> ToResource(FocusSessionDto session)
    {
        var links = new List<LinkDto>
        {
            new(Url.Action(nameof(GetByIdAsync), new { id = session.Id })!, "self", HttpMethods.Get),
            new(Url.Action(nameof(StartAsync), new { id = session.Id })!, "start", HttpMethods.Put),
            new(Url.Action(nameof(CompleteAsync), new { id = session.Id })!, "complete", HttpMethods.Put),
            new(Url.Action(nameof(CancelAsync), new { id = session.Id })!, "cancel", HttpMethods.Put),
            new(Url.Action(nameof(AddBreakAsync), new { id = session.Id })!, "add-break", HttpMethods.Post),
            new(Url.Action(nameof(RecordDeviceSignalAsync), new { id = session.Id })!, "record-device-signal", HttpMethods.Post),
            new(Url.Action(nameof(DeleteAsync), new { id = session.Id })!, "delete", HttpMethods.Delete)
        };

        return new Resource<FocusSessionDto>
        {
            Data = session,
            Links = links
        };
    }

    private IReadOnlyCollection<LinkDto> BuildPaginationLinks(SearchFocusSessionsRequest request, PagedResultDto<FocusSessionDto> result)
    {
        var links = new List<LinkDto>
        {
            new(GenerateSearchUrl(request.PageNumber, request.PageSize), "self", HttpMethods.Get)
        };

        if (result.PageNumber > 1)
        {
            links.Add(new(GenerateSearchUrl(result.PageNumber - 1, result.PageSize), "previous", HttpMethods.Get));
        }

        if (result.PageNumber < result.TotalPages)
        {
            links.Add(new(GenerateSearchUrl(result.PageNumber + 1, result.PageSize), "next", HttpMethods.Get));
        }

        return links;
    }

    private string GenerateSearchUrl(int pageNumber, int pageSize)
    {
        var query = HttpContext.Request.Query
            .Where(pair => !string.Equals(pair.Key, nameof(SearchFocusSessionsRequest.PageNumber), StringComparison.OrdinalIgnoreCase)
                           && !string.Equals(pair.Key, nameof(SearchFocusSessionsRequest.PageSize), StringComparison.OrdinalIgnoreCase))
            .Select(pair =>
            {
                var value = pair.Value.ToString() ?? string.Empty;
                return $"{pair.Key}={Uri.EscapeDataString(value)}";
            })
            .ToList();

        query.Add($"{nameof(SearchFocusSessionsRequest.PageNumber)}={pageNumber}");
        query.Add($"{nameof(SearchFocusSessionsRequest.PageSize)}={pageSize}");

        var baseUrl = Url.Action(nameof(SearchAsync)) ?? "/api/focus-sessions/search";
        var queryString = string.Join("&", query);
        return query.Count > 0 ? $"{baseUrl}?{queryString}" : baseUrl;
    }
}

