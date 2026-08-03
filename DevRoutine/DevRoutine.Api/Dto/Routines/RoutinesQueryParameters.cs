using System.ComponentModel.DataAnnotations;
using DevRoutine.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevRoutine.Api.Dto.Routines;

public sealed record RoutinesQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public RoutineType? Type { get; init; }
    public RoutineStatus? Status { get; init; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;
    [Range(1, 100)]
    public int PageSize { get; init; } = 10;
}
