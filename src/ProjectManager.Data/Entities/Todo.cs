using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data.Entities;
[Table(nameof(Todo))]
public class Todo : ITrackable

{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsDone { get; set; }

    public Instant CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public Instant ModifiedAt { get; set; }
    public string ModifiedBy { get; set; } = null!;
    public Instant? DeletedAt { get; set; }
    public string? DeletedBy { get; set; } = null!;
}
public static class TodoExtensions
{
    public static IQueryable<Todo> FilterDeleted(this IQueryable<Todo> query)
        => query.Where(x => x.DeletedAt == null);
}
