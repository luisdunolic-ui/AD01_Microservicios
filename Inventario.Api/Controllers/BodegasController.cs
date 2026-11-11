using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BodegasController : ControllerBase
{
    private readonly AppDbContext _db;
    public BodegasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bodega>>> GetAll() => await _db.Bodegas.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Bodega>> GetById(int id)
    {
        var item = await _db.Bodegas.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Bodega>> Create(Bodega item)
    {
        item.IdBodega = 0;
        _db.Bodegas.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdBodega }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Bodega item)
    {
        if (id != item.IdBodega) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Bodegas.FindAsync(id);
        if (actual is null) return NotFound();
        actual.Nombre = item.Nombre;
        actual.Ciudad = item.Ciudad;
        actual.Direccion = item.Direccion;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Bodegas.FindAsync(id);
        if (item is null) return NotFound();
        var tieneDatos = await _db.Existencias.AnyAsync(x => x.IdBodega == id)
            || await _db.MovimientoInventarios.AnyAsync(x => x.IdBodega == id)
            || await _db.ReservaStocks.AnyAsync(x => x.IdBodega == id);
        if (tieneDatos) return Conflict("No se puede eliminar la bodega porque tiene inventario, movimientos o reservas asociados.");
        _db.Bodegas.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
