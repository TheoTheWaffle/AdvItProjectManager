using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using ProjectManager.Api;
using ProjectManager.Api.Controllers.Models.Todos;
using ProjectManager.Data.Entities;
using ProjectManager.Data.Interfaces;

namespace ProjectManager.Api.Controllers
{
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly IClock _clock;
        private readonly ApplicationDbContext _Dbcontext;
        public TodoController(ILogger<TodoController> logger, IClock clock, ApplicationDbContext dbcontext)
        {
            _clock = clock;
            _logger = logger;
            _Dbcontext = dbcontext;
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
                ProjectId = model.ProjectId,
                Title = model.Title,
                Description = model.Description,
            }.SetCreateBySystem(now);

            _Dbcontext.Add(newTodo);
            await _Dbcontext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("api/v1/Todo")]
        public async Task<ActionResult<IEnumerable<TodoDetailModel>>> GetList()
        {
            var dbEntities = await _Dbcontext
                .Set<Todo>()
                .FilterDeleted()
                .Select(x => new TodoDetailModel
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    Description = x.Description,
                    Title = x.Title,
                    CreatedAt = x.CreatedAt.ToString(),

                })
                .ToListAsync();

            return Ok(dbEntities);
        }
        [HttpGet("api/v1/Todo/{id}")]
        public async Task<ActionResult> Get(
            [FromRoute] Guid id
            )
        {
            var dbEntity = await _Dbcontext
                .Set<Todo>()
                .Where(X => X.Id == id)
                .FilterDeleted()
                .Select(x => new TodoDetailModel
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
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
        [HttpPatch("api/v1/Todo/{id}")]
        public async Task<ActionResult> Update(
      [FromRoute] Guid id,
      [FromBody]TodoCreateModel patch
      )
        {
            var dbEntity = await _Dbcontext
                .Set<Todo>()
                 .FilterDeleted()
                .SingleOrDefaultAsync(X => X.Id == id);

            if (dbEntity == null)
            {
                return NotFound();
            }

           //some code for update entity

            return NoContent();
        }
        [HttpDelete("api/v1/Todo/{id}")]
        public async Task<ActionResult> Delete(
           [FromRoute] Guid id
           )
        {
            var dbEntity = await _Dbcontext
                .Set<Todo>()
                 .FilterDeleted()
                .SingleOrDefaultAsync(X => X.Id == id);

            if (dbEntity == null)
            {
                return NotFound();
            }

            dbEntity.SetDeleteBySystem(_clock.GetCurrentInstant());
            await _Dbcontext.SaveChangesAsync();

            return NoContent();
        }
    }
}