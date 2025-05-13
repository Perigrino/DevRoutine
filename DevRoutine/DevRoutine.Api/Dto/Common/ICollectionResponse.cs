namespace DevRoutine.Api.Dto.Common;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
