using DevRoutine.Api.Dto.Common;

namespace DevRoutine.Api.Dto.Tags;

public class TagsCollectionDto : ICollectionResponse <TagDto>
{
    public List<TagDto> Items { get; init; } = new();
}
