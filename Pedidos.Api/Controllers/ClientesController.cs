using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Api.Data;
using Pedidos.Api.Models;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClientesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAll() => await _db.Clientes.ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetById(int id)
    {
        var item = await _db.Clientes.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Create(Cliente item)
    {
        item.IdCliente = 0;
        if (await _db.Clientes.AnyAsync(x => x.Cedula == item.Cedula)) return Conflict("La cédula ya está registrada.");
        if (await _db.Clientes.AnyAsync(x => x.Correo == item.Correo)) return Conflict("El correo ya está registrado.");
        _db.Clientes.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.IdCliente }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Cliente item)
    {
        if (id != item.IdCliente) return BadRequest("El ID de la URL no coincide con el ID del objeto.");
        var actual = await _db.Clientes.FindAsync(id);
        if (actual is null) return NotFound();
        if (await _db.Clientes.AnyAsync(x => x.Cedula == item.Cedula && x.IdCliente != id)) return Conflict("La cédula ya está registrada.");
        if (await _db.Clientes.AnyAsync(x => x.Correo == item.Correo && x.IdCliente != id)) return Conflict("El correo ya está registrado.");
        actual.Nombres = item.Nombres;
        actual.Cedula = item.Cedula;
        actual.Correo = item.Correo;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.Clientes.FindAsync(id);
        if (item is null) return NotFound();
        if (await _db.Pedidos.AnyAsync(x => x.IdCliente == id))
            return Conflict("No se puede eliminar el cliente porque tiene pedidos asociados.");
        _db.Clientes.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
