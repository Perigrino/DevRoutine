using System.Linq.Expressions;
using DevRoutine.Api.Entities;

namespace DevRoutine.Api.Dto.Routines;

public static class RoutineMappings 
{
    public static RoutinesDto ToDto(this Routine routine)
    {
        return new RoutinesDto
        {
            Id = routine.Id,
            Name = routine.Name,
            Description = routine.Description,
            Type = routine.Type,
            Frequency = new FrequencyDto
            {
                Type = routine.Frequency.Type,
                TimesPerPeriod = routine.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = routine.Target.Value,
                Unit = routine.Target.Unit
            },
            Status = routine.Status,
            IsArchived = routine.IsArchived,
            EndDate = routine.EndDate,
            Milestone = routine.Milestone != null
                ? new MilestoneDto
                {
                    Target = routine.Milestone.Target,
                    Current = routine.Milestone.Current
                }
                : null,
            CreatedAt = routine.CreatedAt,
            UpdatedAt = routine.UpdatedAt,
            LastCompletedAt = routine.LastCompletedAt
        };
    }
    public static Routine ToEntity(this CreateRoutineDto dto)
    {
        Routine routine = new ()
        {
            Id = $"r_{Guid.CreateVersion7()}",
            Name = dto.Name,
            Description = dto.Description,
            Type = dto.Type,
            Frequency = new Frequency
            {
                Type = dto.Frequency.Type,
                TimesPerPeriod = dto.Frequency.TimesPerPeriod
            },
            Target = new Target
            {
                Value = dto.Target.Value,
                Unit = dto.Target.Unit
            },
            Status = RoutineStatus.Ongoing,
            IsArchived = false,
            EndDate = dto.EndDate,
            Milestone = dto.Milestone is not null
                ? new Milestone
                {
                    Target = dto.Milestone.Target,
                    Current = 0
                }
                : null,
            CreatedAt = DateTime.UtcNow,
        };
        return routine;
    }
    
}
