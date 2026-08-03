using DevRoutine.Api.Entities;
using FluentValidation;

namespace DevRoutine.Api.Dto.Routines;

public abstract class RoutineValidator<T> : AbstractValidator<T> where T : IRoutineInput
{
    protected static readonly string[] AllowedUnits =
    [
        "minutes", "hours", "steps", "km", "cal",
        "pages", "books", "tasks", "sessions"
    ];
    private static readonly string[] AllowedUnitsForBinaryRoutines = ["sessions", "tasks"];

    protected RoutineValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("Habit name must be between 3 and 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid habit type");

        RuleFor(x => x.Frequency.Type)
            .IsInEnum()
            .WithMessage("Invalid frequency period");

        RuleFor(x => x.Frequency.TimesPerPeriod)
            .GreaterThan(0)
            .WithMessage("Frequency must be greater than 0");

        RuleFor(x => x.Target.Value)
            .GreaterThan(0)
            .WithMessage("Target value must be greater than 0");

        RuleFor(x => x.Target.Unit)
            .NotEmpty()
            .Must(unit => string.IsNullOrEmpty(unit) || AllowedUnits.Contains(unit.ToLowerInvariant()))
            .WithMessage($"Unit must be one of: {string.Join(", ", AllowedUnits)}");

        RuleFor(x => x.EndDate)
            .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("End date must be in the future");

        RuleFor(x => x.Target.Unit)
            .Must((dto, unit) => IsTargetUnitCompatibleWithType(dto.Type, unit))
            .WithMessage("Target unit is not compatible with the habit type");
    }

    private static bool IsTargetUnitCompatibleWithType(RoutineType type, string? unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
        {
            return false;
        }

        string normalizedUnit = unit.ToLowerInvariant();

        return type switch
        {
            RoutineType.Binary => AllowedUnitsForBinaryRoutines.Contains(normalizedUnit),
            RoutineType.Measurable => AllowedUnits.Contains(normalizedUnit),
            _ => false
        };
    }
}
