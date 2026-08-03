using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Tags;
using DevRoutine.Api.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevRoutine.Api.Tests;

public sealed class TagValidatorTests
{
    [Fact]
    public async Task CreateValidator_WithValidTag_Passes()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateTagDtoValidation(db);

        var result = await validator.ValidateAsync(new CreateTagDto { Name = "fitness", Description = "workout" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateValidator_WithDuplicateName_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        db.Tags.Add(new Tag { Id = "t_1", Name = "fitness", CreatedAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var validator = new CreateTagDtoValidation(db);
        var result = await validator.ValidateAsync(new CreateTagDto { Name = "fitness" });

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateValidator_WithShortName_Fails()
    {
        await using ApplicationDbContext db = CreateDbContext();
        var validator = new CreateTagDtoValidation(db);

        var result = await validator.ValidateAsync(new CreateTagDto { Name = "ab" });

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateValidator_WithValidTag_Passes()
    {
        var validator = new UpdateTagDtoValidator();

        var result = await validator.ValidateAsync(new UpdateTagDto { Name = "fitness" });

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateValidator_WithEmptyName_Fails()
    {
        var validator = new UpdateTagDtoValidator();

        var result = await validator.ValidateAsync(new UpdateTagDto { Name = string.Empty });

        result.IsValid.Should().BeFalse();
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"tags-{Guid.NewGuid()}")
            .Options;
        return new ApplicationDbContext(options);
    }
}
