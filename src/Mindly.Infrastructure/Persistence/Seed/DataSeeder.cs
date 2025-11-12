using Microsoft.EntityFrameworkCore;
using Mindly.Domain.Entities;
using Mindly.Domain.Enums;
using Mindly.Domain.ValueObjects;

namespace Mindly.Infrastructure.Persistence.Seed;

public class DataSeeder
{
    private readonly MindlyDbContext _dbContext;

    public DataSeeder(MindlyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _dbContext.UserProfiles.AnyAsync(cancellationToken))
        {
            return;
        }

        var remoteUser = new UserProfile(
            "Mindly Remote Worker",
            "remote.worker@mindly.dev",
            WorkMode.Remote,
            Duration.FromMinutes(260));

        var hybridUser = new UserProfile(
            "Mindly Hybrid Leader",
            "hybrid.leader@mindly.dev",
            WorkMode.Hybrid,
            Duration.FromMinutes(210));

        SeedSessions(remoteUser);
        SeedSessions(hybridUser);

        await _dbContext.UserProfiles.AddRangeAsync(new[] { remoteUser, hybridUser }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void SeedSessions(UserProfile user)
    {
        var now = DateTimeOffset.UtcNow;

        var morningSession = user.ScheduleSession(
            "Bloco de Estratégia",
            "Sessão intensa para análise de métricas semanais.",
            FocusMode.DeepWork,
            Duration.FromMinutes(90),
            now.AddMinutes(-240));

        morningSession.Start(now.AddMinutes(-220));
        morningSession.AddBreak(BreakType.Mindfulness, Duration.FromMinutes(10), now.AddMinutes(-180), "Respiração guiada");
        morningSession.RecordDeviceSignal("MindlyLamp", "ambient-light", "{ \"level\": \"focus\" }", now.AddMinutes(-220));
        morningSession.RecordDeviceSignal("MindlyLamp", "ambient-light", "{ \"level\": \"break\" }", now.AddMinutes(-180));
        morningSession.Complete(now.AddMinutes(-140));

        var afternoonSession = user.ScheduleSession(
            "Sprint Pomodoro",
            "Execução de tarefas curtas com foco total.",
            FocusMode.Pomodoro,
            Duration.FromMinutes(50),
            now.AddMinutes(-120));

        afternoonSession.Start(now.AddMinutes(-115));
        afternoonSession.AddBreak(BreakType.Short, Duration.FromMinutes(5), now.AddMinutes(-85));
        afternoonSession.RecordDeviceSignal("MindlyLEDStrip", "color", "{ \"value\": \"#00FF7F\" }", now.AddMinutes(-115));
        afternoonSession.Complete(now.AddMinutes(-60));

        var upcomingSession = user.ScheduleSession(
            "Planejamento semanal",
            "Planejar prioridades da próxima semana.",
            FocusMode.TimeBlocking,
            Duration.FromMinutes(45),
            now.AddMinutes(60));

        upcomingSession.RecordDeviceSignal("MindlyLamp", "ambient-light", "{ \"level\": \"scheduled\" }", now);
    }
}

