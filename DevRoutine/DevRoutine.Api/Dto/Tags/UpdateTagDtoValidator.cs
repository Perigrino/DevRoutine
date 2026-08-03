using FluentValidation;

namespace DevRoutine.Api.Dto.Tags;

public sealed class UpdateTagDtoValidator : TagValidator<UpdateTagDto>
{
    public UpdateTagDtoValidator() : base()
    {
    }
}
