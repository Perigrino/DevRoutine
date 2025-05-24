using System.Linq.Expressions;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Services.Sorting;

namespace DevRoutine.Api.Dto.Routines;

internal static class RoutineMappings
{
    public static readonly SortMappingDefinition<RoutinesDto, Routine> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(RoutinesDto.Name), nameof(Routine.Name)),
            new SortMapping(nameof(RoutinesDto.Description), nameof(Routine.Description)),
            new SortMapping(nameof(RoutinesDto.Type), nameof(Routine.Type)),
            new SortMapping(
                $"{nameof(RoutinesDto.Frequency)}.{nameof(FrequencyDto.Type)}",
                $"{nameof(Routine.Frequency)}.{nameof(Frequency.Type)}"),
            new SortMapping(
                $"{nameof(RoutinesDto.Frequency)}.{nameof(FrequencyDto.TimesPerPeriod)}",
                $"{nameof(Routine.Frequency)}.{nameof(Frequency.TimesPerPeriod)}"),
            new SortMapping(
                $"{nameof(RoutinesDto.Target)}.{nameof(TargetDto.Value)}",
                $"{nameof(Routine.Target)}.{nameof(Target.Value)}"),
            new SortMapping(
                $"{nameof(RoutinesDto.Target)}.{nameof(TargetDto.Unit)}",
                $"{nameof(Routine.Target)}.{nameof(Target.Unit)}"),
            new SortMapping(nameof(RoutinesDto.Status), nameof(Routine.Status)),
            new SortMapping(nameof(RoutinesDto.EndDate), nameof(Routine.EndDate)),
            new SortMapping(nameof(RoutinesDto.CreatedAt), nameof(Routine.CreatedAt)),
            new SortMapping(nameof(RoutinesDto.UpdatedAt), nameof(Routine.UpdatedAt)),
            new SortMapping(nameof(RoutinesDto.LastCompletedAt), nameof(Routine.LastCompletedAt))
        ]
    };

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
    } // Convert DTO to Entity

    public static void UpdateFromDto(this Routine routine, UpdateRoutineDto dto)
    {
        routine.Name = dto.Name;
        routine.Description = dto.Description;
        routine.Type = dto.Type;
        routine.EndDate = dto.EndDate;
        routine.UpdatedAt = DateTime.UtcNow;
        routine.Frequency = new Frequency
        {
            Type = dto.Frequency.Type,
            TimesPerPeriod = dto.Frequency.TimesPerPeriod
        };
        routine.Target = new Target
        {
            Value = dto.Target.Value,
            Unit = dto.Target.Unit
        };
        if (dto.Milestone != null)
        {
            routine.Milestone ??= new Milestone(); // Create new milestone if it doesn't exist
            routine.Milestone.Target = dto.Milestone.Target;
        }
    }
    
}
