using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario.Api.Data;
using Inventario.Api.Models;

namespace Inventario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservaStocksController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReservaStocksController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservaStock>>> GetAll() => await _db.ReservaStocks.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservaStock>> GetById(int id)
    {
        var item = await _db.ReservaStocks.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ReservaStock>> Create(ReservaStock item)
    {
        item.IdReserva = 0;
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        if (string.IsNullOrWhiteSpace(item.Estado)) item.Estado = "Pendiente";
        _db.ReservaStocks.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdReserva }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ReservaStock item)
    {
        if (id != item.IdReserva) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.ReservaStocks.FindAsync(id);
        if (actual is null) return NotFound();
        if (!await _db.Bodegas.AnyAsync(x => x.IdBodega == item.IdBodega))
            return BadRequest("La bodega indicada no existe.");
        actual.IdProducto = item.IdProducto;
        actual.IdBodega = item.IdBodega;
        actual.Cantidad = item.Cantidad;
        actual.Estado = item.Estado;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ReservaStocks.FindAsync(id);
        if (item is null) return NotFound();
        _db.ReservaStocks.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
