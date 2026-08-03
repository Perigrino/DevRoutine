using FluentValidation;

namespace DevRoutine.Api.Dto.RoutineTags;

public sealed class UpsertRoutineTagsDtoValidator : AbstractValidator<UpsertRoutineTagsDto>
{
    public UpsertRoutineTagsDtoValidator()
    {
        RuleFor(x => x.TagIds)
            .NotNull()
            .WithMessage("TagIds is required")
            .NotEmpty()
            .WithMessage("At least one tag id is required")
            .Must(ids => ids is null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Tag ids must be unique");
    }
}
