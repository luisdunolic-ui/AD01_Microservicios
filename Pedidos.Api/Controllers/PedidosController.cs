using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;
    public PedidosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pedido>>> GetAll() => await _db.Pedidos.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Pedido>> GetById(int id)
    {
        var item = await _db.Pedidos.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Pedido>> Create(Pedido item)
    {
        item.IdPedido = 0;
        if (!await _db.Clientes.AnyAsync(x => x.IdCliente == item.IdCliente))
            return BadRequest("El cliente indicado no existe.");
        if (item.Fecha == default) item.Fecha = DateTime.Now;
        item.Total = 0;
        if (string.IsNullOrWhiteSpace(item.Estado)) item.Estado = "Registrado";
        _db.Pedidos.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdPedido }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Pedido item)
    {
        if (id != item.IdPedido) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Pedidos.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Clientes.AnyAsync(x => x.IdCliente == item.IdCliente))
            return BadRequest("El cliente indicado no existe.");
        actual.IdCliente = item.IdCliente;
        actual.Fecha = item.Fecha == default ? actual.Fecha : item.Fecha;
        actual.Estado = item.Estado;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Pedidos.FindAsync(id);
        if (item is null) return NotFound();
        _db.Pedidos.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
