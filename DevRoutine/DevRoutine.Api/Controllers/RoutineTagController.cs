using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.RoutineTags;
using DevRoutine.Api.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;

[Route("routines/{routineId}/tags")]
[ApiController]
public sealed class RoutineTagController(ApplicationDbContext dbContext) : ControllerBase
{
    // PUT api/routines/{routineId}/tags
    // // Updates or inserts tags for a specific routine
    [HttpPut]    
    public async Task<ActionResult> UpsertRoutineTags(string routineId, UpsertRoutineTagsDto upsertRoutineTagsDtos)
    {
        // Retrieve the routine with its associated tags
        Routine? routine = await dbContext.Routines
            .Include(r => r.RoutineTags)
            .FirstOrDefaultAsync(r => r.Id == routineId);

        if (routine is null)
        {
            return NotFound();
        }
        
        // Get the current tag IDs associated with the routine
        var currentTagIds = routine.RoutineTags
            .Select(ht => ht.TagId)
            .ToHashSet();
        
        // Check if the current tags match the provided tags; if so, no update is needed
        if (currentTagIds.SetEquals(upsertRoutineTagsDtos.TagIds))
        {
            return NoContent();
        }
        
        // Retrieve the existing tag IDs from the database
        List<string> existingTagIds = await dbContext.Tags
            .Where(t => upsertRoutineTagsDtos.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        // Validate that all provided tag IDs exist in the database
        if (existingTagIds.Count != upsertRoutineTagsDtos.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid");
        }

        // Remove tags that are no longer associated with the routine
        routine.RoutineTags.RemoveAll(ht => !upsertRoutineTagsDtos.TagIds.Contains(ht.TagId));

        // Add new tags that are not already associated with the routine
        string[] tagIdsToAdd = upsertRoutineTagsDtos.TagIds.Except(currentTagIds).ToArray();
        routine.RoutineTags.AddRange(tagIdsToAdd.Select(tagId => new RoutineTag
        {
            RoutineId = routine.Id,
            TagId = tagId,
            CreatedAt = DateTime.UtcNow
        }));

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteRoutineTag(string routineId, string tagId)
    {
        RoutineTag? habitTag = await dbContext.RoutineTags
            .SingleOrDefaultAsync(rt => rt.RoutineId == routineId && rt.TagId == tagId);

        if (habitTag is null)
        {
            return NotFound();
        }

        dbContext.RoutineTags.Remove(habitTag);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
