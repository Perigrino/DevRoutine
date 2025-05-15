using DevRoutine.Api.Entities;

namespace DevRoutine.Api.Dto.Routines;

public sealed record RoutineWithTagssDto
{
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required RoutineType Type { get; init; }
        public required FrequencyDto Frequency { get; init; }
        public required TargetDto Target { get; init; }
        public required RoutineStatus Status { get; init; }
        public required bool IsArchived { get; init; }
        public DateOnly? EndDate { get; init; }
        public MilestoneDto? Milestone { get; init; }
        public required DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? LastCompletedAt { get; init; }
        public required string[] Tags { get; init; }
}
