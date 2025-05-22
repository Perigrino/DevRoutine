using DevRoutine.Api.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Dto.Tags;

public sealed class CreateTagDtoValidation : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidation(ApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext dbContext = applicationDbContext;
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must be less than 100 characters")
            .MinimumLength(3)
            .WithMessage("Must be more than 3 characters")
            .MustAsync(async (name, cancellationToken) => 
                !await dbContext.Tags.AnyAsync(t => t.Name == name, cancellationToken))
            .WithMessage("A tag with the same name already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be less than 500 characters");
    }
}
