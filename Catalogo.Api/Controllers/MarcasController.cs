using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalogo.Api.Data;
using Catalogo.Api.Models;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarcasController : ControllerBase
{
    private readonly AppDbContext _db;
    public MarcasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Marca>>> GetAll() => await _db.Marcas.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Marca>> GetById(int id)
    {
        var item = await _db.Marcas.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Marca>> Create(Marca item)
    {
        item.IdMarca = 0;
        if (await _db.Marcas.AnyAsync(x => x.Nombre == item.Nombre))
            return Conflict("Ya existe una marca con ese nombre.");
        _db.Marcas.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdMarca }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Marca item)
    {
        if (id != item.IdMarca) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Marcas.FindAsync(id);
        if (actual is null) return NotFound();
        if (await _db.Marcas.AnyAsync(x => x.Nombre == item.Nombre && x.IdMarca != id))
            return Conflict("Ya existe una marca con ese nombre.");
        actual.Nombre = item.Nombre;
        actual.PaisOrigen = item.PaisOrigen;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Marcas.FindAsync(id);
        if (item is null) return NotFound();
        if (await _db.Productos.AnyAsync(x => x.IdMarca == id))
            return Conflict("No se puede eliminar la marca porque tiene productos asociados.");
        _db.Marcas.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
