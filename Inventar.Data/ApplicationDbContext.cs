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
    public DbSet<Material> Materials { get; set; }

    public DbSet<Capacity> Capacities { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<RecurringExpense> RecurringExpenses { get; set; }
    public DbSet<ProductionCenter> ProductionCenters { get; set; }
    public DbSet<ProductionCenterStorage> ProductionCenterStorages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductionCenter>().ToTable("ProductionCenters", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<Capacity>()
            .HasIndex(bc => new { bc.PrimaryMaterialBaseId, bc.MaterialId })
            .IsUnique();

        modelBuilder.Entity<Capacity>()
            .Property(bc => bc.CapacityLimit)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<StockTransaction>()
            .Property(st => st.QuantityChange)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<StockTransaction>()
            .HasOne(st => st.Base)
            .WithMany(b => b.StockTransactions)
            .HasForeignKey(st => st.BaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockTransaction>()
            .HasOne(st => st.Material)
            .WithMany()
            .HasForeignKey(st => st.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Base)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.BaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecurringExpense>()
            .Property(re => re.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<RecurringExpense>()
            .HasOne(re => re.Base)
            .WithMany(b => b.RecurringExpenses)
            .HasForeignKey(re => re.BaseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
