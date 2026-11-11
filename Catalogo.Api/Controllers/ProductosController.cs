using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Catalogo.Api.Data;
using Catalogo.Api.Models;

namespace Catalogo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll() => await _db.Productos.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var item = await _db.Productos.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> Create(Producto item)
    {
        item.IdProducto = 0;
        if (!await _db.Categorias.AnyAsync(x => x.IdCategoria == item.IdCategoria))
            return BadRequest("La categoría indicada no existe.");
        if (!await _db.Marcas.AnyAsync(x => x.IdMarca == item.IdMarca))
            return BadRequest("La marca indicada no existe.");
        if (await _db.Productos.AnyAsync(x => x.SKU == item.SKU))
            return Conflict("Ya existe un producto con ese SKU.");
        _db.Productos.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdProducto }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Producto item)
    {
        if (id != item.IdProducto) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Productos.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Categorias.AnyAsync(x => x.IdCategoria == item.IdCategoria))
            return BadRequest("La categoría indicada no existe.");
        if (!await _db.Marcas.AnyAsync(x => x.IdMarca == item.IdMarca))
            return BadRequest("La marca indicada no existe.");
        if (await _db.Productos.AnyAsync(x => x.SKU == item.SKU && x.IdProducto != id))
            return Conflict("Ya existe un producto con ese SKU.");

        actual.IdCategoria = item.IdCategoria;
        actual.IdMarca = item.IdMarca;
        actual.Nombre = item.Nombre;
        actual.SKU = item.SKU;
        actual.Precio = item.Precio;
        actual.Estado = item.Estado;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Productos.FindAsync(id);
        if (item is null) return NotFound();
        _db.Productos.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
