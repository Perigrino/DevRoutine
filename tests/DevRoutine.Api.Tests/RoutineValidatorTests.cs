using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevRoutine.Api.Tests;

public sealed class RoutineValidatorTests
{
    [Fact]
    public async Task CreateValidator_WithValidDto_Passes()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateRoutineDtoValidator(db);

        var result = await validator.ValidateAsync(CreateValidDto());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateValidator_WithDuplicateName_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        db.Routines.Add(CreateRoutine("Morning Run"));
        await db.SaveChangesAsync();

        var validator = new CreateRoutineDtoValidator(db);
        var result = await validator.ValidateAsync(CreateValidDto() with { Name = "Morning Run" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateValidator_WithShortName_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateRoutineDtoValidator(db);

        var result = await validator.ValidateAsync(CreateValidDto() with { Name = "ab" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateValidator_WithIncompatibleUnitForBinaryType_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateRoutineDtoValidator(db);

        var result = await validator.ValidateAsync(CreateValidDto() with
        {
            Type = RoutineType.Binary,
            Target = new TargetDto { Value = 1, Unit = "km" }
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task CreateValidator_WithNegativeMilestoneTarget_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateRoutineDtoValidator(db);

        var result = await validator.ValidateAsync(CreateValidDto() with
        {
            Milestone = new MilestoneDto { Target = 0, Current = 0 }
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateValidator_WithValidDto_Passes()
    {
        var validator = new UpdateRoutineDtoValidator();

        var result = await validator.ValidateAsync(CreateValidUpdateDto());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateValidator_WithInvalidTargetValue_Fails()
    {
        var validator = new UpdateRoutineDtoValidator();

        var result = await validator.ValidateAsync(new UpdateRoutineDto
        {
            Name = "Morning Run",
            Type = RoutineType.Measurable,
            Frequency = new FrequencyDto { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
            Target = new TargetDto { Value = 0, Unit = "km" }
        });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateValidator_WithEmptyName_Fails()
    {
        var validator = new UpdateRoutineDtoValidator();

        var result = await validator.ValidateAsync(new UpdateRoutineDto
        {
            Name = string.Empty,
            Type = RoutineType.Measurable,
            Frequency = new FrequencyDto { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
            Target = new TargetDto { Value = 5, Unit = "km" }
        });

        result.IsValid.Should().BeFalse();
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"routines-{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }

    private static CreateRoutineDto CreateValidDto() => new()
    {
        Name = "Morning Run",
        Type = RoutineType.Measurable,
        Frequency = new FrequencyDto { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
        Target = new TargetDto { Value = 5, Unit = "km" }
    };

    private static UpdateRoutineDto CreateValidUpdateDto() => new()
    {
        Name = "Morning Run",
        Type = RoutineType.Measurable,
        Frequency = new FrequencyDto { Type = FrequencyType.Daily, TimesPerPeriod = 1 },
        Target = new TargetDto { Value = 5, Unit = "km" }
    };

    private static Routine CreateRoutine(string name) => new()
    {
        Id = $"r_{Guid.NewGuid():N}",
        Name = name,
        Frequency = new Frequency(),
        Target = new Target { Unit = "km" },
        CreatedAt = DateTime.UtcNow,
        RoutineTags = [],
        Tags = []
    };
}
