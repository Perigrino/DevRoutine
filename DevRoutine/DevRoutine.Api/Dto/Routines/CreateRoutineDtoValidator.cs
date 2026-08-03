using DevRoutine.Api.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Dto.Routines;

public sealed class CreateRoutineDtoValidator : RoutineValidator<CreateRoutineDto>
{
    public CreateRoutineDtoValidator(ApplicationDbContext dbContext) : base()
    {
        When(x => x.Milestone is not null, () =>
        {
            RuleFor(x => x.Milestone!.Target)
                .GreaterThan(0)
                .WithMessage("Milestone target must be greater than 0");
        });

        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) =>
                !await dbContext.Routines.AnyAsync(r => r.Name == name, cancellationToken))
            .WithMessage("A routine with the same name already exists");
    }
}
