using System.Dynamic;
using DevRoutine.Api.Dto.Common;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Services;
using FluentAssertions;
using Xunit;

namespace DevRoutine.Api.Tests;

public sealed class DataShapingServiceTests
{
    private readonly DataShapingService _service = new();

    [Fact]
    public void ShapeData_WithoutFields_ReturnsAllPublicProperties()
    {
        ExpandoObject shaped = _service.ShapeData(CreateRoutinesDto(), null);

        IDictionary<string, object?> dict = shaped;
        dict.Keys.Should().Contain(["Id", "Name", "Type", "Frequency", "CreatedAt"]);
    }

    [Fact]
    public void ShapeData_WithFields_ReturnsOnlyRequestedFields()
    {
        ExpandoObject shaped = _service.ShapeData(CreateRoutinesDto(), "id,name");

        IDictionary<string, object?> dict = shaped;
        dict.Keys.Should().BeEquivalentTo("Id", "Name");
    }

    [Fact]
    public void ShapeData_WithUnknownField_ReturnsOnlyKnownFields()
    {
        ExpandoObject shaped = _service.ShapeData(CreateRoutinesDto(), "id,nonexistent");

        IDictionary<string, object?> dict = shaped;
        dict.Keys.Should().BeEquivalentTo("Id");
    }

    [Fact]
    public void Validate_WithNullFields_ReturnsTrue()
    {
        _service.Validate<RoutinesDto>(null).Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidFields_ReturnsTrue()
    {
        _service.Validate<RoutinesDto>("id,name,type").Should().BeTrue();
    }

    [Fact]
    public void Validate_WithUnknownFields_ReturnsFalse()
    {
        _service.Validate<RoutinesDto>("id,unknown").Should().BeFalse();
    }

    [Fact]
    public void ShapeCollectionData_WithoutFactory_OmitsLinks()
    {
        List<ExpandoObject> shaped = _service.ShapeCollectionData([CreateRoutinesDto()], null);

        IDictionary<string, object?> first = shaped[0];
        first.Should().NotContainKey("links");
    }

    [Fact]
    public void ShapeCollectionData_WithFactory_AddsLinksPerItem()
    {
        List<ExpandoObject> shaped = _service.ShapeCollectionData(
            [CreateRoutinesDto(), CreateRoutinesDto()],
            null,
            _ => [new LinkDto { Href = "/routines/1", Rel = "self", Method = "GET" }]);

        shaped.Should().HaveCount(2);
        IDictionary<string, object?> first = shaped[0];
        first.Should().ContainKey("links");
    }

    private static RoutinesDto CreateRoutinesDto() => new()
    {
        Id = "r_1",
        Name = "Morning Run",
        Type = RoutineType.Measurable,
        Frequency = new FrequencyDto { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
        Target = new TargetDto { Value = 5, Unit = "km" },
        Status = RoutineStatus.Ongoing,
        IsArchived = false,
        CreatedAt = DateTime.UtcNow
    };
}
