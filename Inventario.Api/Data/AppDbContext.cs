using Microsoft.EntityFrameworkCore;
using Inventario.Api.Models;

namespace Inventario.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Bodega> Bodegas => Set<Bodega>();
    public DbSet<Existencia> Existencias => Set<Existencia>();
    public DbSet<MovimientoInventario> MovimientoInventarios => Set<MovimientoInventario>();
    public DbSet<ReservaStock> ReservaStocks => Set<ReservaStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bodega>().ToTable("Bodega");
        modelBuilder.Entity<Existencia>().ToTable("Existencia");
        modelBuilder.Entity<MovimientoInventario>().ToTable("MovimientoInventario");
        modelBuilder.Entity<ReservaStock>().ToTable("ReservaStock");

        modelBuilder.Entity<Existencia>().HasIndex(x => new { x.IdProducto, x.IdBodega }).IsUnique();
        modelBuilder.Entity<Existencia>().HasOne<Bodega>().WithMany()
            .HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<MovimientoInventario>().HasOne<Bodega>().WithMany()
            .HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ReservaStock>().HasOne<Bodega>().WithMany()
            .HasForeignKey(x => x.IdBodega).OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
