using DevRoutine.Api.Dto.RoutineTags;
using FluentAssertions;
using Xunit;

namespace DevRoutine.Api.Tests;

public sealed class UpsertRoutineTagsDtoValidatorTests
{
    [Fact]
    public async Task Validate_WithValidTagIds_Passes()
    {
        var validator = new UpsertRoutineTagsDtoValidator();

        var result = await validator.ValidateAsync(new UpsertRoutineTagsDto { TagIds = ["t_1", "t_2"] });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithNullTagIds_Fails()
    {
        var validator = new UpsertRoutineTagsDtoValidator();

        var result = await validator.ValidateAsync(new UpsertRoutineTagsDto { TagIds = null! });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyTagIds_Fails()
    {
        var validator = new UpsertRoutineTagsDtoValidator();

        var result = await validator.ValidateAsync(new UpsertRoutineTagsDto { TagIds = [] });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithDuplicateTagIds_Fails()
    {
        var validator = new UpsertRoutineTagsDtoValidator();

        var result = await validator.ValidateAsync(new UpsertRoutineTagsDto { TagIds = ["t_1", "t_1"] });

        result.IsValid.Should().BeFalse();
    }
}
