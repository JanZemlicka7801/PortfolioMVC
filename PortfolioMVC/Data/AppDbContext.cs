using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>().HasOne(p => p.Manager).WithMany(m => m.Projects).HasForeignKey(p => p.ManagerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeamMember>().HasOne(t => t.Project).WithMany().HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}