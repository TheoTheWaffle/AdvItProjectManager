namespace ProjectManager.Api.Controllers.Models.Todos
{
    public class TodoCreateModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public Guid ProjectId { get; set; }

    }
}
