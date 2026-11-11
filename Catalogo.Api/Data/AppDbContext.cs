using Microsoft.EntityFrameworkCore;
using Catalogo.Api.Models;

namespace Catalogo.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().ToTable("Categoria");
        modelBuilder.Entity<Marca>().ToTable("Marca");
        modelBuilder.Entity<Producto>().ToTable("Producto");

        modelBuilder.Entity<Categoria>().HasIndex(x => x.Nombre).IsUnique();
        modelBuilder.Entity<Marca>().HasIndex(x => x.Nombre).IsUnique();
        modelBuilder.Entity<Producto>().HasIndex(x => x.SKU).IsUnique();

        modelBuilder.Entity<Producto>().HasOne<Categoria>().WithMany()
            .HasForeignKey(x => x.IdCategoria).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Producto>().HasOne<Marca>().WithMany()
            .HasForeignKey(x => x.IdMarca).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Producto>().Property(x => x.Precio).HasPrecision(10, 2);

        base.OnModelCreating(modelBuilder);
    }
}
