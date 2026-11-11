using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExistenciasController : ControllerBase
{
    private readonly AppDbContext _db;
    public ExistenciasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Existencia>>> GetAll() => await _db.Existencias.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Existencia>> GetById(int id)
    {
        var item = await _db.Existencias.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Existencia>> Create(Existencia item)
    {
        item.IdExistencia = 0;
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        if (await _db.Existencias.AnyAsync(x => x.IdProducto == item.IdProducto && x.IdBodega == item.IdBodega))
            return Conflict("Ya existe una existencia para ese producto en esa bodega.");
        _db.Existencias.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdExistencia }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Existencia item)
    {
        if (id != item.IdExistencia) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Existencias.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        if (await _db.Existencias.AnyAsync(x => x.IdProducto == item.IdProducto && x.IdBodega == item.IdBodega && x.IdExistencia != id))
            return Conflict("Ya existe una existencia para ese producto en esa bodega.");
        actual.IdProducto = item.IdProducto;
        actual.IdBodega = item.IdBodega;
        actual.Stock = item.Stock;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Existencias.FindAsync(id);
        if (item is null) return NotFound();
        _db.Existencias.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
