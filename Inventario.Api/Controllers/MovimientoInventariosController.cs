using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimientoInventariosController : ControllerBase
{
    private readonly AppDbContext _db;
    public MovimientoInventariosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoInventario>>> GetAll() => await _db.MovimientoInventarios.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovimientoInventario>> GetById(int id)
    {
        var item = await _db.MovimientoInventarios.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<MovimientoInventario>> Create(MovimientoInventario item)
    {
        item.IdMovimiento = 0;
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        if (item.Fecha == default) item.Fecha = DateTime.Now;
        _db.MovimientoInventarios.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdMovimiento }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, MovimientoInventario item)
    {
        if (id != item.IdMovimiento) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.MovimientoInventarios.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        actual.IdProducto = item.IdProducto;
        actual.IdBodega = item.IdBodega;
        actual.Tipo = item.Tipo;
        actual.Cantidad = item.Cantidad;
        actual.Fecha = item.Fecha == default ? actual.Fecha : item.Fecha;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.MovimientoInventarios.FindAsync(id);
        if (item is null) return NotFound();
        _db.MovimientoInventarios.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
