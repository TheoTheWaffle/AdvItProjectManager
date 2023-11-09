using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Todo> Todo { get; set; } = null!;
        public DbSet<Project> Project { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions options): base(options)
        {

        }
    }
}
