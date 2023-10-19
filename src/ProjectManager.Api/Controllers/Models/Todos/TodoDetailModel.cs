namespace ProjectManager.Api.Controllers.Models.Todos;

public class TodoDetailModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string CreatedAt { get; set; } = null!;

}
