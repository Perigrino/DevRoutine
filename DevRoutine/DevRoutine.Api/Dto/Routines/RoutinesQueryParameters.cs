using DevRoutine.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenTelemetry.Trace;

namespace DevRoutine.Api.Dto.Routines;

public sealed record RoutinesQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public RoutineType? Type { get; init; }
    public RoutineStatus? Status { get; init; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
