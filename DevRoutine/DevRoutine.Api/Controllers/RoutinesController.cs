using System.Dynamic;
using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Common;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Services;
using DevRoutine.Api.Services.Sorting;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;

[ApiController]
[Route("routines")]
public sealed class RoutinesController(
    ApplicationDbContext dbContext,
    LinkService linkService,
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    IValidator<CreateRoutineDto> createRoutineValidator,
    IValidator<UpdateRoutineDto> updateRoutineValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoutines([FromQuery] RoutinesQueryParameters query, CancellationToken cancellationToken)
    {
        if (!sortMappingProvider.ValidateMappings<RoutinesDto, Routine>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'");
        }

        if (!dataShapingService.Validate<RoutinesDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{query.Fields}'");
        }

        query.Search = query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<RoutinesDto, Routine>();

        IQueryable<RoutinesDto> routinesQuery = dbContext.Routines
            .Where(r => query.Search == null ||
                        r.Name.ToLower().Contains(query.Search) ||
                        r.Description != null && r.Description.ToLower().Contains(query.Search))
            .Where(r => query.Type == null || r.Type == query.Type)
            .Where(r => query.Status == null || r.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(RoutineQueries.ProjectToDto());

        PaginationResult<RoutinesDto> paginated = await PaginationResult<RoutinesDto>.CreateAsync(
            routinesQuery, query.Page, query.PageSize, cancellationToken);

        var paginationResult = new PaginationResult<ExpandoObject>
        {
            Items = dataShapingService.ShapeCollectionData(
                paginated.Items, query.Fields, r => CreateLinkForRoutine(r.Id, query.Fields)),
            Page = paginated.Page,
            PageSize = paginated.PageSize,
            TotalCount = paginated.TotalCount
        };
        paginationResult.Links = CreateLinkForRoutines(query, paginationResult.HasNextPage, paginationResult.HasPreviousPage);
        return Ok(paginationResult);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoutineWithTagssDto>> GetRoutine(string id, string? fields, CancellationToken cancellationToken)
    {
        if (!dataShapingService.Validate<RoutineWithTagssDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{fields}'");
        }

        RoutineWithTagssDto? routine = await dbContext.Routines
            .Where(r => r.Id == id)
            .Select(RoutineQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync(cancellationToken);
        if (routine is null)
        {
            return NotFound();
        }

        ExpandoObject shapedRoutineDto = dataShapingService.ShapeData(routine, fields);
        List<LinkDto> links = CreateLinkForRoutine(id, fields);
        shapedRoutineDto.TryAdd("links", links);
        return Ok(shapedRoutineDto);
    }

    [HttpPost]
    public async Task<ActionResult<RoutinesDto>> CreateRoutine(CreateRoutineDto createRoutineDto, CancellationToken cancellationToken)
    {
        await createRoutineValidator.ValidateAndThrowAsync(createRoutineDto, cancellationToken);

        Routine routine = createRoutineDto.ToEntity();
        dbContext.Add(routine);
        await dbContext.SaveChangesAsync(cancellationToken);
        RoutinesDto routinesDto = routine.ToDto();
        routinesDto.Links = CreateLinkForRoutine(routine.Id, null);
        return CreatedAtAction(nameof(GetRoutine), new { id = routine.Id }, routinesDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRoutine(string id, UpdateRoutineDto updateRoutineDto, CancellationToken cancellationToken)
    {
        await updateRoutineValidator.ValidateAndThrowAsync(updateRoutineDto, cancellationToken);

        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (routine is null)
        {
            return NotFound();
        }
        routine.UpdateFromDto(updateRoutineDto);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchRoutine(string id, JsonPatchDocument<RoutinesDto> patchDocument, CancellationToken cancellationToken)
    {
        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (routine is null)
        {
            return NotFound();
        }
        RoutinesDto routineDto = routine.ToDto();
        patchDocument.ApplyTo(routineDto, ModelState);
        if (!TryValidateModel(routineDto))
        {
            return ValidationProblem(ModelState);
        }
        routine.Name = routineDto.Name;
        routine.Description = routineDto.Description;
        routine.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoutine(string id, CancellationToken cancellationToken)
    {
        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        if (routine is null)
        {
            return NotFound();
        }
        dbContext.Remove(routine);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private List<LinkDto> CreateLinkForRoutine(string id, string? fields)
    {
        List<LinkDto> links =
        [
            linkService.Create(nameof(GetRoutine), "self", HttpMethods.Get, new { id, fields }),
            linkService.Create(nameof(GetRoutine), "update", HttpMethods.Put, new { id }),
            linkService.Create(nameof(GetRoutine), "patch-update", HttpMethods.Patch, new { id }),
            linkService.Create(nameof(GetRoutine), "delete", HttpMethods.Delete, new { id }),
            linkService.Create(nameof(RoutineTagController.UpsertRoutineTags), "upsert-tags", HttpMethods.Put,
                new { routineId = id }, RoutineTagController.Name)
        ];
        return links;
    }

    private List<LinkDto> CreateLinkForRoutines(RoutinesQueryParameters parameters, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links =
            [
                linkService.Create(nameof(GetRoutines), "self", HttpMethods.Get, new
                {
                    q = parameters.Search,
                    type = parameters.Type,
                    status = parameters.Status,
                    sort = parameters.Sort,
                    page = parameters.Page,
                    pageSize = parameters.PageSize,
                    fields = parameters.Fields
                }),
                linkService.Create(nameof(CreateRoutine), "create", HttpMethods.Post, null)
            ];
        if (hasNextPage)
        {
            links.Add(linkService.Create(nameof(GetRoutines), "next-page", HttpMethods.Get, new
            {
                q = parameters.Search,
                type = parameters.Type,
                status = parameters.Status,
                sort = parameters.Sort,
                page = parameters.Page + 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields
            }));
        }
        if (hasPreviousPage)
        {
            links.Add(linkService.Create(nameof(GetRoutines), "previous-page", HttpMethods.Get, new
            {
                q = parameters.Search,
                type = parameters.Type,
                status = parameters.Status,
                sort = parameters.Sort,
                page = parameters.Page - 1,
                pageSize = parameters.PageSize,
                fields = parameters.Fields
            }));
        }

        return links;
    }
}
