using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.RoutineTags;
using DevRoutine.Api.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;

[Route("routines/{routineId}/tags")]
[ApiController]
public sealed class RoutineTagController(
    ApplicationDbContext dbContext,
    IValidator<UpsertRoutineTagsDto> upsertRoutineTagsValidator) : ControllerBase
{
    public static readonly string Name = nameof(RoutineTagController).Replace("Controller", string.Empty);

    [HttpPut]
    public async Task<ActionResult> UpsertRoutineTags(string routineId, UpsertRoutineTagsDto upsertRoutineTagsDtos, CancellationToken cancellationToken)
    {
        await upsertRoutineTagsValidator.ValidateAndThrowAsync(upsertRoutineTagsDtos, cancellationToken);

        Routine? routine = await dbContext.Routines
            .Include(r => r.RoutineTags)
            .FirstOrDefaultAsync(r => r.Id == routineId, cancellationToken);

        if (routine is null)
        {
            return NotFound();
        }

        var currentTagIds = routine.RoutineTags
            .Select(ht => ht.TagId)
            .ToHashSet();

        if (currentTagIds.SetEquals(upsertRoutineTagsDtos.TagIds))
        {
            return NoContent();
        }

        List<string> existingTagIds = await dbContext.Tags
            .Where(t => upsertRoutineTagsDtos.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync(cancellationToken);

        if (existingTagIds.Count != upsertRoutineTagsDtos.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid");
        }

        routine.RoutineTags.RemoveAll(ht => !upsertRoutineTagsDtos.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = upsertRoutineTagsDtos.TagIds.Except(currentTagIds).ToArray();
        routine.RoutineTags.AddRange(tagIdsToAdd.Select(tagId => new RoutineTag
        {
            RoutineId = routine.Id,
            TagId = tagId,
            CreatedAt = DateTime.UtcNow
        }));

        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteRoutineTag(string routineId, string tagId, CancellationToken cancellationToken)
    {
        RoutineTag? routineTag = await dbContext.RoutineTags
            .SingleOrDefaultAsync(rt => rt.RoutineId == routineId && rt.TagId == tagId, cancellationToken);

        if (routineTag is null)
        {
            return NotFound();
        }

        dbContext.RoutineTags.Remove(routineTag);

        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
