using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Tags;
using DevRoutine.Api.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;

[Route("tags")]
[ApiController]
public sealed class TagController(
    ApplicationDbContext dbContext,
    IValidator<CreateTagDto> createTagValidator,
    IValidator<UpdateTagDto> updateTagValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags(CancellationToken cancellationToken)
    {
        List<TagDto> tags = await dbContext
            .Tags
            .Select(TagQueries.ProjectToDto())
            .ToListAsync(cancellationToken);

        var tagsCollectionDto = new TagsCollectionDto
        {
            Items = tags
        };

        return Ok(tagsCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id, CancellationToken cancellationToken)
    {
        TagDto? tagDto = await dbContext.Tags
            .Where(t => t.Id == id)
            .Select(TagQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);
        if (tagDto is null)
        {
            return NotFound();
        }
        return Ok(tagDto);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createTagDto, CancellationToken cancellationToken)
    {
        await createTagValidator.ValidateAndThrowAsync(createTagDto, cancellationToken);

        Tag tag = createTagDto.ToEntity();
        dbContext.Add(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        TagDto tagDto = tag.ToDto();
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TagDto>> UpdateTag(string id, UpdateTagDto updateTagDto, CancellationToken cancellationToken)
    {
        await updateTagValidator.ValidateAndThrowAsync(updateTagDto, cancellationToken);

        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag is null)
        {
            return NotFound();
        }
        tag.UpdateFromDto(updateTagDto);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id, CancellationToken cancellationToken)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (tag is null)
        {
            return NotFound();
        }
        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
