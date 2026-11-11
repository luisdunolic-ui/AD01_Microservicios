using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Api.Models;

[Table("MovimientoInventario")]
public class MovimientoInventario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdMovimiento { get; set; }

    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdBodega { get; set; }

    [Required]
    [RegularExpression("^(Entrada|Salida)$")]
    [StringLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}
