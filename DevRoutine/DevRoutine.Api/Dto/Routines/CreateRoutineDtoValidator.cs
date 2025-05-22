using DevRoutine.Api.Database;
using DevRoutine.Api.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Dto.Routines;

public sealed class CreateRoutineDtoValidator : AbstractValidator<CreateRoutineDto>
{
    private static readonly string[] AllowedUnits =
    [
        "minutes", "hours", "steps", "km", "cal",
        "pages", "books", "tasks", "sessions"
    ];
    private static readonly string[] AllowedUnitsForBinaryHabits = ["sessions", "tasks"];

    public CreateRoutineDtoValidator(ApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext dbContext = applicationDbContext;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("Habit name must be between 3 and 100 characters")
            .MustAsync(async (name, cancellationToken) => 
                !await dbContext.Routines.AnyAsync(r => r.Name == name, cancellationToken))
            .WithMessage("A routine with the same name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid habit type");

        // Frequency validation
        RuleFor(x => x.Frequency.Type)
            .IsInEnum()
            .WithMessage("Invalid frequency period");

        RuleFor(x => x.Frequency.TimesPerPeriod)
            .GreaterThan(0)
            .WithMessage("Frequency must be greater than 0");

        // Target validation
        RuleFor(x => x.Target.Value)
            .GreaterThan(0)
            .WithMessage("Target value must be greater than 0");

        RuleFor(x => x.Target.Unit)
            .NotEmpty()
            .Must(unit => AllowedUnits.Contains(unit.ToLowerInvariant()))
            .WithMessage($"Unit must be one of: {string.Join(", ", AllowedUnits)}");

        // EndDate validation
        RuleFor(x => x.EndDate)
            .Must(date => date is null || date.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("End date must be in the future");

        // Milestone validation
        When(x => x.Milestone is not null, () =>
        {
            RuleFor(x => x.Milestone!.Target)
                .GreaterThan(0)
                .WithMessage("Milestone target must be greater than 0");
        });

        // Complex rules
        RuleFor(x => x.Target.Unit)
            .Must((dto, unit) => IsTargetUnitCompatibleWithType(dto.Type, unit))
            .WithMessage("Target unit is not compatible with the habit type");
        
    }
    
    
    private static bool IsTargetUnitCompatibleWithType(RoutineType type, string unit)
    {
        string normalizedUnit = unit.ToLowerInvariant();

        return type switch
        {
            // Binary routines should only use count-based units
            RoutineType.Binary => AllowedUnitsForBinaryHabits.Contains(normalizedUnit),
            // Measurable routines can use any of the allowed units
            RoutineType.Measurable => AllowedUnits.Contains(normalizedUnit),
            _ => false // None is not valid
        };
    }
}
