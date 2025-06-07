using System.Dynamic;
using System.Linq.Dynamic.Core;
using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Migrations.Application;
using DevRoutine.Api.Services;
using DevRoutine.Api.Services.Sorting;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;
[ApiController]
[Route("routines")]

public sealed class RoutinesController(ApplicationDbContext dbContext) : ControllerBase
{
    // This method retrieves a list of routines based on query parameters such as sorting, filtering, and pagination.
    // It also validates the provided sort and data shaping fields, applies sorting, and shapes the data before returning it.
    //GET api/<RoutineController>
    [HttpGet]
    public async Task<IActionResult> GetRoutines([FromQuery] RoutinesQueryParameters query,
        SortMappingProvider sortMappingProvider, DataShapingService dataShapingService)
    {
        // Validate the sort parameter against the sort mappings
        if (!sortMappingProvider.ValidateMappings<RoutinesDto, Routine>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'");
        }
        
        // Validate the fields parameter for data shaping
        if (!dataShapingService.Validate<RoutinesDto>(query.Fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{query.Fields}'");
        }

        // Trim and normalize the search query
        query.Search ??= query.Search?.Trim().ToLower();

        // Retrieve the sort mappings for the DTO and entity
        SortMapping[] sortMappings = sortMappingProvider.GetMappings<RoutinesDto, Routine>();

        // Build the query with filtering, sorting, and projection to DTO
        IQueryable<RoutinesDto> routinesQuery = dbContext.Routines
            .Where(r => query.Search == null ||
                        r.Name.ToLower().Contains(query.Search) ||
                        r.Description != null && r.Description.ToLower().Contains(query.Search))
            .Where(r => query.Type == null || r.Type == query.Type)
            .Where(r => query.Status == null || r.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(RoutineQueries.ProjectToDto());
        
        // Get the total count of routines for pagination
        int totalCount = await routinesQuery.CountAsync();

        // Apply pagination to the query
        List<RoutinesDto> routine = await routinesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // Shape the data based on the requested fields
        var paginationResult = new PaginationResult<ExpandoObject>
        {
            Items = dataShapingService.ShapeCollectionData(routine, query.Fields),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
        return Ok(paginationResult);
    }

    //GET api/<RoutineController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoutineWithTagssDto>> GetRoutine(string id, string? fields, DataShapingService dataShapingService)
    {
        if (!dataShapingService.Validate<RoutineWithTagssDto>(fields))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided data shaping fields aren't valid: '{fields}'");
        }
        RoutineWithTagssDto? routine = await dbContext.Routines
            .Where(r => r.Id ==id)
            .Select(RoutineQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync();
        if (routine is null)
        {
            return NotFound();
        }
        
        ExpandoObject shapedRoutineDto = dataShapingService.ShapeData(routine, fields);
        return Ok(shapedRoutineDto);
    }
    
    // POST api/<RoutineController>
    [HttpPost]
    public async Task<ActionResult<RoutinesDto>> CreateRoutine(CreateRoutineDto createRoutineDto, IValidator<CreateRoutineDto> validator)
    {
        
        // Validate the CreateRoutineDto object and throw an exception if validation fails
        await validator.ValidateAndThrowAsync(createRoutineDto);
        
        Routine routine = createRoutineDto.ToEntity(); // Convert DTO to Entity
        dbContext.Add(routine);
        await dbContext.SaveChangesAsync();
        RoutinesDto routinesDto = routine.ToDto(); // Convert Entity to DTO
        return CreatedAtAction(nameof(GetRoutine), new { id = routine.Id }, routinesDto);
    }
    
    // PUT api/<RoutineController>/5
    [HttpPut("{id}")]    
    public async Task<ActionResult> UpdateRoute(string id, UpdateRoutineDto updateRoutineDto)
    {
        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id);
        if (routine is null)
        {
            return NotFound();
        }
        routine.UpdateFromDto(updateRoutineDto);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    // PATCH api/<RoutineController>/5
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchRoutine(string id, JsonPatchDocument<RoutinesDto> patchDocument)
    {
        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id);
        if (routine is null)
        {
            return NotFound();
        }
        RoutinesDto routineDto = routine.ToDto(); // Convert Entity to DTO
        patchDocument.ApplyTo(routineDto, ModelState);
        if (!TryValidateModel(routineDto))
        {
            return ValidationProblem(ModelState);
        }
        routine.Name = routineDto.Name;
        routine.Description = routineDto.Description;
        routine.UpdatedAt = routineDto.UpdatedAt;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    // DELETE api/<RoutineController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRoutine (string id)
    {
        Routine? routine = await dbContext.Routines.FirstOrDefaultAsync(r => r.Id == id);
        if (routine is null)
        {
            return NotFound();
        }
        dbContext.Remove(routine);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    
}
