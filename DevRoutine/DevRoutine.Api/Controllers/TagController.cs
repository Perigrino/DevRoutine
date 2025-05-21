using DevRoutine.Api.Database;
using DevRoutine.Api.Dto.Tags;
using DevRoutine.Api.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevRoutine.Api.Controllers;

[Route("tags")]
[ApiController]
public sealed class TagsController(ApplicationDbContext dbContext) : ControllerBase
{
    //GET: api/<TagController>
    [HttpGet]
    public async Task<ActionResult<TagsCollectionDto>> GetTags()
    {
        List<TagDto> tags = await dbContext
            .Tags
            .Select(TagQueries.ProjectToDto())
            .ToListAsync();

        var habitsCollectionDto = new TagsCollectionDto
        {
            Items = tags
        };

        return Ok(habitsCollectionDto);
    }

    // GET api/<TagController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        TagDto? tagDto = await dbContext.Tags
            .Where(t => t.Id == id)
            .Select(TagQueries.ProjectToDto())
            .FirstOrDefaultAsync();
        if (tagDto is null)
        {
            return NotFound();
        }
        return Ok(tagDto);
    }
    
    
    // POST api/<TagController>
    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(
        CreateTagDto createTagDto, IValidator<CreateTagDto> validator)
    {
        await validator.ValidateAsync(createTagDto);
        
        Tag tag = createTagDto.ToEntity(); // Convert DTO to Entity
        if (await dbContext.Tags.AnyAsync(r => r.Name == createTagDto.Name))
        {
            return Problem(
                detail: $"The tag with name '{createTagDto.Name}' already exists",
                statusCode: StatusCodes.Status409Conflict);
        }

        dbContext.Add(tag);
        await dbContext.SaveChangesAsync();
        TagDto tagDto = tag.ToDto(); // Convert Entity to DTO
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
    }


    // PUT api/<TagController>/5
    [HttpPut("{id}")] 
    public async Task<ActionResult<TagDto>> UpdateTag(string id, UpdateTagDto updateTagDto)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (tag is null)
        {
            return NotFound();
        }
        tag.UpdateFromDto(updateTagDto);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
    
    // DELETE api/<TagController>/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTag(string id)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
        if (tag is null)
        {
            return NotFound();
        }
        dbContext.Tags.Remove(tag);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
