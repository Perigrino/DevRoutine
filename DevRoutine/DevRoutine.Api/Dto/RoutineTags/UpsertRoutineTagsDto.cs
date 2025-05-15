namespace DevRoutine.Api.Dto.RoutineTags;

public sealed record UpsertRoutineTagsDto
{
    public required List<string> TagIds { get; set; }
}
