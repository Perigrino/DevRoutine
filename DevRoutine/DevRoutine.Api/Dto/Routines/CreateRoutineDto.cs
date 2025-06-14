using DevRoutine.Api.Entities;

namespace DevRoutine.Api.Dto.Routines;

public sealed record CreateRoutineDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required RoutineType Type { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public required TargetDto Target { get; init; }
    public DateOnly? EndDate { get; init; }
    public MilestoneDto? Milestone { get; init; }
}
