using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using ProjectManager.Api;
using ProjectManager.Api.Controllers.Models.Project;
using ProjectManager.Api.Controllers.Models.Todos;
using ProjectManager.Data.Entities;
using ProjectManager.Data.Interfaces;

namespace ProjectManager.Api.Controllers
{

    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly IClock _clock;
        private readonly ApplicationDbContext _dbContext;
        public ProjectController(ILogger<ProjectController> logger, IClock clock, ApplicationDbContext dbcontext)
        {
            _clock = clock;
            _logger = logger;
            _dbContext = dbcontext;
        }


        [HttpPost("api/v1/Controller")]
        public async Task<ActionResult> Create(
           [FromBody] ProjectCreateModel model
           )
        {
            var now = _clock.GetCurrentInstant();
            
            var newController = new Project
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
            }.SetCreateBySystem(now);


            var uniqueCheck = await _dbContext.Set<Todo>().AnyAsync(x => x.Title == newController.Title);
            if(uniqueCheck )
            {
                return BadRequest("Title is not unique");
            }
            _dbContext.Add(newController);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("api/v1/Project")]
        public async Task<ActionResult<IEnumerable<ProjectDetailModel>>> GetList()
        {
            var dbEntities = await _dbContext
                .Set<Project>()
                .FilterDeleted()
                .Select(x => new ProjectDetailModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Title = x.Title,
                    CreatedAt = x.CreatedAt.ToString(),
                })
                .ToListAsync();

            return Ok(dbEntities);
        }
        [HttpGet("api/v1/Project/{id}")]
        public async Task<ActionResult> Get(
            [FromRoute] Guid id
            )
        {
            var dbEntity = await _dbContext
                .Set<Project>()
                .Where(X => X.Id == id)
                .FilterDeleted()
                .Select(x => new ProjectDetailModel
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
        [HttpPatch("api/v1/Project/{id}")]
        public async Task<ActionResult> Update(
      [FromRoute] Guid id,
      [FromBody] ProjectCreateModel patch
      )
        {
            var dbEntity = await _dbContext
                .Set<Project>()
                 .FilterDeleted()
                .SingleOrDefaultAsync(X => X.Id == id);

            if (dbEntity == null)
            {
                return NotFound();
            }

            //some code for update entity

            return NoContent();
        }
        [HttpDelete("api/v1/Project/{id}")]
        public async Task<ActionResult> Delete(
           [FromRoute] Guid id
           )
        {
            var dbEntity = await _dbContext
                .Set<Project>()
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