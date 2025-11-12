using Mindly.Application.Common;
using Mindly.Application.DTOs.Sessions;

namespace Mindly.Application.Services.Contracts;

public interface IFocusSessionService
{
    Task<FocusSessionDto> CreateAsync(CreateFocusSessionRequest request, CancellationToken cancellationToken = default);
    Task<FocusSessionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FocusSessionDto> StartAsync(Guid id, StartFocusSessionRequest request, CancellationToken cancellationToken = default);
    Task<FocusSessionDto> CompleteAsync(Guid id, CompleteFocusSessionRequest request, CancellationToken cancellationToken = default);
    Task<FocusSessionDto> CancelAsync(Guid id, CancelFocusSessionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BreakPeriodDto> AddBreakAsync(Guid id, AddBreakRequest request, CancellationToken cancellationToken = default);
    Task<DeviceSignalDto> RecordDeviceSignalAsync(Guid id, RecordDeviceSignalRequest request, CancellationToken cancellationToken = default);
    Task<PagedResultDto<FocusSessionDto>> SearchAsync(SearchFocusSessionsRequest request, CancellationToken cancellationToken = default);
}

