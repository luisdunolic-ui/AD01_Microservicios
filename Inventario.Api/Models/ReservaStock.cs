using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario.Api.Models;

[Table("ReservaStock")]
public class ReservaStock
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdReserva { get; set; }

    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdBodega { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Required]
    [RegularExpression("^(Pendiente|Confirmada|Cancelada)$")]
    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";
}
