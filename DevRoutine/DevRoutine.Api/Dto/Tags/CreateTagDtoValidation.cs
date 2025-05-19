using FluentValidation;

namespace DevRoutine.Api.Dto.Tags;

public sealed class CreateTagDtoValidation : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must be less than 100 characters")
            .MinimumLength(3)
            .WithMessage("Must be more than 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be less than 500 characters");
    }
}
