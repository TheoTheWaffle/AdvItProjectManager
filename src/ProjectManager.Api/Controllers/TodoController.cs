using Microsoft.AspNetCore.JsonPatch;
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
        private readonly ApplicationDbContext _dbContext;
        public TodoController(ILogger<TodoController> logger, IClock clock, ApplicationDbContext dbcontext)
        {
            _clock = clock;
            _logger = logger;
            _dbContext = dbcontext;
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
                    ProjectId = x.ProjectId,
                    Description = x.Description,
                    Title = x.Title,
                    CreatedAt = InstantPattern.ExtendedIso.Format(x.CreatedAt),

                })
                .ToListAsync();

            return Ok(dbEntities);
        }
        [HttpGet("api/v1/Todo/{id}")]
        public async Task<ActionResult> Get(
            [FromRoute] Guid id
            )
        {
            var dbEntity = await _dbContext
                .Set<Todo>()
                .Where(X => X.Id == id)
                .FilterDeleted()
                .Select(x => new TodoDetailModel
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    Description = x.Description,
                    Title = x.Title,
                    CreatedAt = InstantPattern.ExtendedIso.Format(x.CreatedAt),
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
      [FromBody] JsonPatchDocument<TodoCreateModel> patch
      )
        {
            var dbEntity = await _dbContext
                .Set<Todo>()
                 .FilterDeleted()
                .SingleOrDefaultAsync(X => X.Id == id);

            if (dbEntity == null)
            {
                return NotFound();
            }
            var now = _clock.GetCurrentInstant();
            var toUpdate = new TodoCreateModel
            {
                Description = dbEntity.Description,
                Title = dbEntity.Title,
            };
            patch.ApplyTo(toUpdate);

            if (!(ModelState.IsValid && TryValidateModel(toUpdate)))
            {
                return ValidationProblem(ModelState);
            }
            dbEntity.Title = toUpdate.Title;
            dbEntity.Description = toUpdate.Description;
            dbEntity.SetModifyBySystem(now);

            await _dbContext.SaveChangesAsync();
            dbEntity = await _dbContext.Set<Todo>().FirstAsync(x =>x.Id==dbEntity.Id);
            return Ok(new TodoDetailModel
            {
                Id = dbEntity.Id,
                Description = dbEntity.Description,
                Title = dbEntity.Title,
                CreatedAt = InstantPattern.ExtendedIso.Format(dbEntity.CreatedAt),
            }); 
            //some code for update entity

            return NoContent();
        }
        [HttpDelete("api/v1/Todo/{id}")]
        public async Task<ActionResult> Delete(
           [FromRoute] Guid id
           )
        {
            var dbEntity = await _dbContext
                .Set<Todo>()
                 .FilterDeleted()
                .SingleOrDefaultAsync(X => X.Id == id);

            if (dbEntity == null)
            {
                return NotFound();
            }

            dbEntity.SetDeleteBySystem(_clock.GetCurrentInstant());
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}