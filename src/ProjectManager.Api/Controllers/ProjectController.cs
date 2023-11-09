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
        private readonly ApplicationDbContext _Dbcontext;
        public ProjectController(ILogger<ProjectController> logger, IClock clock, ApplicationDbContext dbcontext)
        {
            _clock = clock;
            _logger = logger;
            _Dbcontext = dbcontext;
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

            _Dbcontext.Add(newController);
            await _Dbcontext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("api/v1/Project")]
        public async Task<ActionResult<IEnumerable<ProjectDetailModel>>> GetList()
        {
            var dbEntities = await _Dbcontext
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
            var dbEntity = await _Dbcontext
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
            var dbEntity = await _Dbcontext
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
            var dbEntity = await _Dbcontext
                .Set<Project>()
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