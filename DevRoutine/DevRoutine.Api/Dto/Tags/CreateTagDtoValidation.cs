using DevRoutine.Api.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Dto.Tags;

public sealed class CreateTagDtoValidation : TagValidator<CreateTagDto>
{
    public CreateTagDtoValidation(ApplicationDbContext applicationDbContext) : base()
    {
        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) =>
                !await applicationDbContext.Tags.AnyAsync(t => t.Name == name, cancellationToken))
            .WithMessage("A tag with the same name already exists");
    }
}
