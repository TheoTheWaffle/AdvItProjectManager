using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NodaTime;
using NodaTime.Text;
using ProjectManager.Api.Controllers.Models.Todos;
using ProjectManager.Data;
using ProjectManager.Data.Entities;
using ProjectManager.Data.Interfaces;
using System.Reflection;

namespace ProjectManager.Api.Controllers;

[ApiController]
public class TodoController : ControllerBase
{

    private readonly ILogger<TodoController> _logger;
    private readonly IClock _clock;
    private readonly AppDbContext _dbContext;
    public TodoController(
        ILogger<TodoController> logger,
        IClock clock,
        AppDbContext dbContext
        )
    {
        _clock = clock;
        _logger = logger;
        _dbContext = dbContext;
    }
    [HttpPost("api/v1/Todo")]
    public async Task<ActionResult> Create(
        [FromBody] TodoCreateModel model
        )
    {
        var now = _clock.GetCurrentInstant();
        var newTodo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description,
        }.SetCreatedBySystem(now);

        _dbContext.Add(newTodo);
        await _dbContext.SaveChangesAsync();

        return Ok();

    }
    [HttpGet("api/v1/Todo")]
    public async Task<ActionResult<IEnumerable<TodoDetailModel>>> GetList()
    {
        var dbEntities = await _dbContext
            .Set<Todo>()
            .FilterDeleted()
            .Select(x => new TodoDetailModel
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                CreatedAt = x.CreatedAt.ToString(),
            })
            .ToListAsync();

        return Ok(dbEntities);
    }
    [HttpPatch("api/v1/Todo/{id}")]
    public async Task<ActionResult> Update(
        [FromRoute] Guid id,
        [FromBody]TodoCreateModel patch
        )
    {
        var dbEntity = await _dbContext
            .Set<Todo>()
            .FilterDeleted()
            .SingleOrDefaultAsync(x => x.Id == id);
        if (dbEntity == null)
        {
            return NotFound();
        }

        // some code for update entity
        
        return NoContent();
    }
    [HttpGet("api/v1/Todo/{id}")]
    public async Task<ActionResult<TodoDetailModel>> Get([FromRoute] Guid id)
    {
        var dbEntity = await _dbContext
            .Set<Todo>()
            .FilterDeleted()
            .Where(x => x.Id == id)
            .Select(x => new TodoDetailModel
            {
                Id = x.Id,
                Description = x.Description,
                Title = x.Title,
                CreatedAt = x.CreatedAt.ToString(),
            })
            .SingleOrDefaultAsync();
        if (dbEntity == null)
        {
            return NotFound();
        }
        return Ok(dbEntity);
    }
    [HttpDelete("api/v1/Todo/{id}")]
    public async Task<ActionResult> Delete(
        [FromRoute] Guid id
        )
    {
        var dbEntity = await _dbContext
            .Set<Todo>()
            .SingleOrDefaultAsync(x=>x.Id==id);
        if (dbEntity == null)
        {
            return NotFound();
        }
        dbEntity.SetDeletedBySystem(_clock.GetCurrentInstant());
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}