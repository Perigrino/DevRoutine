namespace DevRoutine.Api.Dto.Tags;

public interface ITagInput
{
    string Name { get; set; }
    string? Description { get; set; }
}
