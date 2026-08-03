using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Services.Sorting;
using FluentAssertions;
using Xunit;

namespace DevRoutine.Api.Tests;

public sealed class SortingTests
{
    [Fact]
    public void ApplySort_WithField_SortsAscending()
    {
        SortMapping[] mappings = CreateDefinition().Mappings;

        string[] names = CreateQueryable().ApplySort("name", mappings).Select(r => r.Name).ToArray();

        names.Should().Equal("Alpha", "Beta");
    }

    [Fact]
    public void ApplySort_WithDescModifier_SortsDescending()
    {
        SortMapping[] mappings = CreateDefinition().Mappings;

        string[] names = CreateQueryable().ApplySort("name desc", mappings).Select(r => r.Name).ToArray();

        names.Should().Equal("Beta", "Alpha");
    }

    [Fact]
    public void ApplySort_WithoutSort_UsesDefaultOrderBy()
    {
        SortMapping[] mappings = CreateDefinition().Mappings;

        string[] ids = CreateQueryable().ApplySort(null, mappings).Select(r => r.Id).ToArray();

        ids.Should().Equal("r_1", "r_2");
    }

    [Fact]
    public void SortMappingProvider_ValidateMappings_AcceptsKnownFields()
    {
        var provider = new SortMappingProvider([CreateDefinition()]);

        provider.ValidateMappings<RoutinesDto, Routine>("name,createdAt desc").Should().BeTrue();
    }

    [Fact]
    public void SortMappingProvider_ValidateMappings_RejectsUnknownFields()
    {
        var provider = new SortMappingProvider([CreateDefinition()]);

        provider.ValidateMappings<RoutinesDto, Routine>("unknown").Should().BeFalse();
    }

    [Fact]
    public void SortMappingProvider_ValidateMappings_NullOrEmpty_ReturnsTrue()
    {
        var provider = new SortMappingProvider([CreateDefinition()]);

        provider.ValidateMappings<RoutinesDto, Routine>(null).Should().BeTrue();
    }

    [Fact]
    public void SortMappingProvider_GetMappings_ThrowsWhenNotRegistered()
    {
        var provider = new SortMappingProvider([]);

        var act = () => provider.GetMappings<RoutinesDto, Routine>();

        act.Should().Throw<InvalidOperationException>();
    }

    private static SortMappingDefinition<RoutinesDto, Routine> CreateDefinition() => new()
    {
        Mappings =
        [
            new SortMapping(nameof(RoutinesDto.Name), nameof(Routine.Name)),
            new SortMapping(nameof(RoutinesDto.CreatedAt), nameof(Routine.CreatedAt))
        ]
    };

    private static IQueryable<Routine> CreateQueryable() =>
        new List<Routine>
        {
            CreateRoutine("r_1", "Beta", new DateTime(2024, 1, 2)),
            CreateRoutine("r_2", "Alpha", new DateTime(2024, 1, 1))
        }.AsQueryable();

    private static Routine CreateRoutine(string id, string name, DateTime createdAt) => new()
    {
        Id = id,
        Name = name,
        Frequency = new Frequency(),
        Target = new Target { Unit = "km" },
        CreatedAt = createdAt,
        RoutineTags = [],
        Tags = []
    };
}
