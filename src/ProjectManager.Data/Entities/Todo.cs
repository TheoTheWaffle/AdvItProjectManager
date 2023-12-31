﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManager.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data.Entities
{
    [Table(nameof(Todo))]
    public class Todo : ITrackable
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool Done { get; set; }

        public Instant CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
        public Instant ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = null!;
        public Instant? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
    }
    public static class ToDoExtensions
    {
        public static IQueryable<Todo> FilterDeleted(this IQueryable<Todo> query)
            => query
            .Where(x => x.DeletedAt == null)
            ;
    }
}
