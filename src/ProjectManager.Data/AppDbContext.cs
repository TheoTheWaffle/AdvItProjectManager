using Microsoft.EntityFrameworkCore;
using ProjectManager.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<Todo> Todos { get; set; } = null!;
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }
}
