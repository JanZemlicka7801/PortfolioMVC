using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Manager> Managers { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>().HasOne(p => p.Manager).WithMany().HasForeignKey(p => p.ManagerId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TeamMember>().HasOne(t => t.Project).WithMany().HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}