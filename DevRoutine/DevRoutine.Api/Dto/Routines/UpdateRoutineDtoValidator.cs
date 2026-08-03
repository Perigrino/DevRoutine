using FluentValidation;

namespace DevRoutine.Api.Dto.Routines;

public sealed class UpdateRoutineDtoValidator : RoutineValidator<UpdateRoutineDto>
{
    public UpdateRoutineDtoValidator() : base()
    {
        When(x => x.Milestone is not null, () =>
        {
            RuleFor(x => x.Milestone!.Target)
                .GreaterThan(0)
                .WithMessage("Milestone target must be greater than 0");
        });
    }
}
