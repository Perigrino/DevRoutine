using System.Linq.Expressions;
using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
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
    public async Task<ActionResult<RoutineDtoCollection>> GetRoutines()
    {
        List<RoutinesDto> routines = await dbContext.Routines
            .Select(RoutineQueries.ProjectToDto()
            ).ToListAsync();

        var routineCollectionDtoDto = new RoutineDtoCollection
        {
            Data = routines
        };
        return Ok(routineCollectionDtoDto);
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
        
        await validator.ValidateAndThrowAsync(createRoutineDto);
        if (await dbContext.Routines.AnyAsync(r => r.Name == createRoutineDto.Name))
        {
            return Problem(
                detail: $"The tag with name '{createRoutineDto.Name}' already exists",
                statusCode: StatusCodes.Status409Conflict);
        }
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
