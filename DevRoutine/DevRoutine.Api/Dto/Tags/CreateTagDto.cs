using FluentValidation;

namespace DevRoutine.Api.Dto.Tags;

public sealed record CreateTagDto : ITagInput
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
