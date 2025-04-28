using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortfolioMVC.Models.entities;

namespace PortfolioMVC.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<TeamMember> TeamMembers { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>().HasOne(p => p.Manager).WithMany(m => m.Projects).HasForeignKey(p => p.ManagerId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TeamMember>().HasOne(t => t.Project).WithMany().HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);

        //Shopping stuff
        modelBuilder.Entity<Product>().HasOne(p => p.CreatedBy).WithMany().HasForeignKey(p => p.CreatedById).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CartItem>().HasOne(c => c.Product).WithMany().HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Order>().HasOne(o => o.User).WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<OrderItem>().HasOne(oi => oi.Product).WithMany().HasForeignKey(oi => oi.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}