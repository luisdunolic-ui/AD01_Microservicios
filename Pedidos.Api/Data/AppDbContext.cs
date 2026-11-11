using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Models;

namespace Pedidos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallePedidos => Set<DetallePedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>().ToTable("Cliente");
        modelBuilder.Entity<Pedido>().ToTable("Pedido");
        modelBuilder.Entity<DetallePedido>().ToTable("DetallePedido");

        modelBuilder.Entity<Cliente>().HasIndex(x => x.Cedula).IsUnique();
        modelBuilder.Entity<Cliente>().HasIndex(x => x.Correo).IsUnique();
        modelBuilder.Entity<Pedido>().HasOne<Cliente>().WithMany()
            .HasForeignKey(x => x.IdCliente).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DetallePedido>().HasOne<Pedido>().WithMany()
            .HasForeignKey(x => x.IdPedido).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Pedido>().Property(x => x.Total).HasPrecision(12, 2);
        modelBuilder.Entity<DetallePedido>().Property(x => x.PrecioUnitario).HasPrecision(10, 2);

        base.OnModelCreating(modelBuilder);
    }
}
