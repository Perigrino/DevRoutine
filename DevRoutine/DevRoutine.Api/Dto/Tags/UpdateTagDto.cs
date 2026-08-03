namespace DevRoutine.Api.Dto.Tags;

public sealed record UpdateTagDto : ITagInput
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
