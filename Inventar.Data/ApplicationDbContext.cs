using Inventar.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventar.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PrimaryMaterialBase> PrimaryMaterialBases { get; set; }
    public DbSet<Capacity> Capacities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PrimaryMaterialBase>()
            .HasMany(b => b.Capacities)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Capacity>()
            .HasOne(c => c.PrimaryMaterialBase)
            .WithMany(b => b.Capacities)
            .HasForeignKey(c => c.PrimaryMaterialBaseId);
    }
}
