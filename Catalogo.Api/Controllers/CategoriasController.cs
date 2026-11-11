using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalogo.Api.Data;
using Catalogo.Api.Models;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;
    public CategoriasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAll() => await _db.Categorias.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Categoria>> GetById(int id)
    {
        var item = await _db.Categorias.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Categoria>> Create(Categoria item)
    {
        item.IdCategoria = 0;
        if (await _db.Categorias.AnyAsync(x => x.Nombre == item.Nombre))
            return Conflict("Ya existe una categoría con ese nombre.");

        _db.Categorias.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdCategoria }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Categoria item)
    {
        if (id != item.IdCategoria) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Categorias.FindAsync(id);
        if (actual is null) return NotFound();
        if (await _db.Categorias.AnyAsync(x => x.Nombre == item.Nombre && x.IdCategoria != id))
            return Conflict("Ya existe una categoría con ese nombre.");

        actual.Nombre = item.Nombre;
        actual.Descripcion = item.Descripcion;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Categorias.FindAsync(id);
        if (item is null) return NotFound();
        if (await _db.Productos.AnyAsync(x => x.IdCategoria == id))
            return Conflict("No se puede eliminar la categoría porque tiene productos asociados.");
        _db.Categorias.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
