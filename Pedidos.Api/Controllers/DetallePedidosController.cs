using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DetallePedidosController : ControllerBase
{
    private readonly AppDbContext _db;
    public DetallePedidosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetallePedido>>> GetAll() => await _db.DetallePedidos.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DetallePedido>> GetById(int id)
    {
        var item = await _db.DetallePedidos.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DetallePedido>> Create(DetallePedido item)
    {
        item.IdDetalle = 0;
        if (!await _db.Pedidos.AnyAsync(x => x.IdPedido == item.IdPedido))
            return BadRequest("El pedido indicado no existe.");

        await using var transaction = await _db.Database.BeginTransactionAsync();
        _db.DetallePedidos.Add(item);
        await _db.SaveChangesAsync();
        await ActualizarTotal(item.IdPedido);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdDetalle }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, DetallePedido item)
    {
        if (id != item.IdDetalle) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.DetallePedidos.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Pedidos.AnyAsync(x => x.IdPedido == item.IdPedido))
            return BadRequest("El pedido indicado no existe.");

        var pedidoAnterior = actual.IdPedido;
        await using var transaction = await _db.Database.BeginTransactionAsync();
        actual.IdPedido = item.IdPedido;
        actual.IdProducto = item.IdProducto;
        actual.Cantidad = item.Cantidad;
        actual.PrecioUnitario = item.PrecioUnitario;
        await _db.SaveChangesAsync();
        await ActualizarTotal(pedidoAnterior);
        if (item.IdPedido != pedidoAnterior) await ActualizarTotal(item.IdPedido);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.DetallePedidos.FindAsync(id);
        if (item is null) return NotFound();
        var idPedido = item.IdPedido;
        await using var transaction = await _db.Database.BeginTransactionAsync();
        _db.DetallePedidos.Remove(item);
        await _db.SaveChangesAsync();
        await ActualizarTotal(idPedido);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();
        return NoContent();
    }

    private async Task ActualizarTotal(int idPedido)
    {
        var pedido = await _db.Pedidos.FindAsync(idPedido);
        if (pedido is null) return;
        pedido.Total = await _db.DetallePedidos
            .Where(x => x.IdPedido == idPedido)
            .SumAsync(x => (decimal?)(x.Cantidad * x.PrecioUnitario)) ?? 0m;
    }
}
