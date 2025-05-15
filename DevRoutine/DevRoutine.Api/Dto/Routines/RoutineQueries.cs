using System.Linq.Expressions;
using DevRoutine.Api.Entities;

namespace DevRoutine.Api.Dto.Routines;

internal static class RoutineQueries
{
    public static Expression<Func<Routine, RoutinesDto>> ProjectToDto()
    {
        return r => new RoutinesDto()
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Type = r.Type, 
            Frequency = new FrequencyDto
            {
                Type = r.Frequency.Type,
                TimesPerPeriod = r.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = r.Target.Value,
                Unit = r.Target.Unit
            },
            Status = r.Status,
            IsArchived = r.IsArchived,
            EndDate = r.EndDate,
            Milestone = r.Milestone != null
                ? new MilestoneDto
                {
                    Target = r.Milestone.Target,
                    Current = r.Milestone.Current
                }
                : null,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            LastCompletedAt = r.LastCompletedAt
        };
    }
    
    public static Expression<Func<Routine, RoutineWithTagssDto>> ProjectToDtoWithTags()
    {
        return r => new RoutineWithTagssDto()
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            Type = r.Type, 
            Frequency = new FrequencyDto
            {
                Type = r.Frequency.Type,
                TimesPerPeriod = r.Frequency.TimesPerPeriod
            },
            Target = new TargetDto
            {
                Value = r.Target.Value,
                Unit = r.Target.Unit
            },
            Status = r.Status,
            IsArchived = r.IsArchived,
            EndDate = r.EndDate,
            Milestone = r.Milestone != null
                ? new MilestoneDto
                {
                    Target = r.Milestone.Target,
                    Current = r.Milestone.Current
                }
                : null,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            LastCompletedAt = r.LastCompletedAt,
            Tags = r.Tags.Select(t => t.Name).ToArray()
        };
    }
}
