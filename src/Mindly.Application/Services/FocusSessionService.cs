using Mindly.Application.Common;
using Mindly.Application.DTOs.Sessions;
using Mindly.Application.Exceptions;
using Mindly.Application.Mappings;
using Mindly.Application.Services.Contracts;
using Mindly.Domain.Abstractions;
using Mindly.Domain.Entities;
using Mindly.Domain.ValueObjects;

namespace Mindly.Application.Services;

public class FocusSessionService : IFocusSessionService
{
    private static readonly HashSet<string> AllowedSortColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "plannedstart",
        "createdat",
        "title",
        "status"
    };

    private readonly IFocusSessionRepository _sessionRepository;
    private readonly IUserProfileRepository _userRepository;
    private readonly IDeviceSignalRepository _deviceSignalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FocusSessionService(
        IFocusSessionRepository sessionRepository,
        IUserProfileRepository userRepository,
        IDeviceSignalRepository deviceSignalRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _deviceSignalRepository = deviceSignalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FocusSessionDto> CreateAsync(CreateFocusSessionRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureUserExists(request.UserProfileId, cancellationToken);

        var session = new FocusSession(
            request.UserProfileId,
            request.Title,
            request.Description,
            request.FocusMode,
            Duration.FromMinutes(request.PlannedDurationMinutes),
            request.PlannedStart);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FocusSessionMapper.ToDto(session);
    }

    public async Task<FocusSessionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        return FocusSessionMapper.ToDto(session);
    }

    public async Task<FocusSessionDto> StartAsync(Guid id, StartFocusSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        session.Start(request.StartedAt);

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FocusSessionMapper.ToDto(session);
    }

    public async Task<FocusSessionDto> CompleteAsync(Guid id, CompleteFocusSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        Duration? actualDuration = null;
        if (request.ActualDurationMinutes.HasValue)
        {
            actualDuration = Duration.FromMinutes(request.ActualDurationMinutes.Value);
        }

        session.Complete(request.CompletedAt, actualDuration);

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FocusSessionMapper.ToDto(session);
    }

    public async Task<FocusSessionDto> CancelAsync(Guid id, CancelFocusSessionRequest request, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        session.Cancel(request.Reason ?? string.Empty);

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return FocusSessionMapper.ToDto(session);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        await _sessionRepository.DeleteAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<BreakPeriodDto> AddBreakAsync(Guid id, AddBreakRequest request, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        var breakPeriod = session.AddBreak(
            request.BreakType,
            Duration.FromMinutes(request.DurationMinutes),
            request.StartedAt,
            request.Notes);

        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BreakPeriodDto
        {
            Id = breakPeriod.Id,
            BreakType = breakPeriod.BreakType,
            DurationMinutes = breakPeriod.Duration.TotalMinutes,
            StartedAt = breakPeriod.StartedAt,
            Notes = breakPeriod.Notes
        };
    }

    public async Task<DeviceSignalDto> RecordDeviceSignalAsync(Guid id, RecordDeviceSignalRequest request, CancellationToken cancellationToken = default)
    {
        var session = await GetSessionAsync(id, cancellationToken);
        var signal = session.RecordDeviceSignal(
            request.DeviceName,
            request.SignalType,
            request.Payload,
            request.RecordedAt);

        await _deviceSignalRepository.AddAsync(signal, cancellationToken);
        await _sessionRepository.UpdateAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeviceSignalDto
        {
            Id = signal.Id,
            DeviceName = signal.DeviceName,
            SignalType = signal.SignalType,
            Payload = signal.Payload,
            RecordedAt = signal.RecordedAt
        };
    }

    public async Task<PagedResultDto<FocusSessionDto>> SearchAsync(SearchFocusSessionsRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            throw new ValidationException(
                "Os parâmetros de paginação são inválidos.",
                new Dictionary<string, string[]>
                {
                    ["pageNumber"] = new[] { "Deve ser maior que zero." },
                    ["pageSize"] = new[] { "Deve ser maior que zero." }
                });
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy) && !AllowedSortColumns.Contains(request.SortBy))
        {
            throw new ValidationException(
                "Campo de ordenação inválido.",
                new Dictionary<string, string[]>
                {
                    ["sortBy"] = new[] { $"Permitidos: {string.Join(", ", AllowedSortColumns)}." }
                });
        }

        var result = await _sessionRepository.SearchAsync(
            request.UserProfileId,
            request.StartFrom,
            request.StartTo,
            request.FocusMode,
            request.Status,
            request.Keyword,
            request.SortBy,
            request.Descending,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dto = new PagedResultDto<FocusSessionDto>
        {
            Items = result.Items.Select(FocusSessionMapper.ToDto).ToArray(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        return dto;
    }

    private async Task EnsureUserExists(Guid userId, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.GetByIdAsync(userId, cancellationToken) is not null;
        if (!exists)
        {
            throw new NotFoundException("Perfil associado não foi encontrado.");
        }
    }

    private async Task<FocusSession> GetSessionAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _sessionRepository.GetByIdAsync(id, cancellationToken)
               ?? throw new NotFoundException("Sessão de foco não encontrada.");
    }
}

