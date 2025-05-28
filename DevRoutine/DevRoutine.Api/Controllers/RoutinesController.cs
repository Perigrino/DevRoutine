using System.Dynamic;
using System.Linq.Dynamic.Core;
using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using DevRoutine.Api.Migrations.Application;
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
    //GET api/<RoutineController>
    [HttpGet]
    public async Task<ActionResult<PaginationResult<RoutinesDto>>> GetRoutines([FromQuery] RoutinesQueryParameters query,
        SortMappingProvider sortMappingProvider)
    {
        if (!sortMappingProvider.ValidateMappings<RoutinesDto, Routine>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'");
        }

        query.Search ??= query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<RoutinesDto, Routine>();

        IQueryable<RoutinesDto> routinesQuery = dbContext.Routines
            .Where(r => query.Search == null ||
                        r.Name.ToLower().Contains(query.Search) ||
                        r.Description != null && r.Description.ToLower().Contains(query.Search))
            .Where(r => query.Type == null || r.Type == query.Type)
            .Where(r => query.Status == null || r.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(RoutineQueries.ProjectToDto());
        
        var paginationResult = await PaginationResult<RoutinesDto>.CreateAsync(routinesQuery, query.Page, query.PageSize);
        
        return Ok(paginationResult);
    }

    //GET api/<RoutineController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoutineWithTagssDto>> GetRoutine(string id)
    {
        RoutineWithTagssDto? routine = await dbContext.Routines
            .Where(r => r.Id ==id)
            .Select(RoutineQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync();
        if (routine is null)
        {
            return NotFound();
        }
        return Ok(routine);
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
