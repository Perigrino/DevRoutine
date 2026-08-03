using DevRoutine.Api.Entities;

namespace DevRoutine.Api.Dto.Routines;

public interface IRoutineInput
{
    string Name { get; }
    string? Description { get; }
    RoutineType Type { get; }
    FrequencyDto Frequency { get; }
    TargetDto Target { get; }
    DateOnly? EndDate { get; }
}
