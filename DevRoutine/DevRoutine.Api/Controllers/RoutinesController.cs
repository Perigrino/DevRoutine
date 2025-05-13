using System.Linq.Expressions;
using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Routines;
using DevRoutine.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;
[ApiController]
[Route("routines")]

public sealed class RoutinesController(ApplicationDbContext dbContext) : ControllerBase
{
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
    public async Task<ActionResult<RoutinesDto>> GetRoutine(string id)
    {
        RoutinesDto? routine = await dbContext.Routines
            .Where(r => r.Id ==id)
            .Select(RoutineQueries.ProjectToDto())
            .FirstOrDefaultAsync();
        if (routine is null)
        {
            return NotFound();
        }
        return Ok(routine);
    }

    
    // POST api/<RoutineController>
    [HttpPost]
    public async Task<ActionResult<RoutinesDto>> CreateRoutine(CreateRoutineDto createRoutine)
    {
        Routine routine = createRoutine.ToEntity();
        dbContext.Add(routine);
        await dbContext.SaveChangesAsync();
        RoutinesDto routinesDto = routine.ToDto();
        return CreatedAtAction(nameof(GetRoutine), new { id = routine.Id }, routinesDto);
    }
    
}

    // // PUT api/<RoutineController>/5
    // [HttpPut("{id}")]    
    // public void Put(int id, [FromBody] string value)
    // {
    // }
    //
    // // DELETE api/<RoutineController>/5
    // [HttpDelete("{id}")]
    // public void Delete(int id)
    // {
    // }
