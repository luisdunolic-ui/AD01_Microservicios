using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Api.Models;

[Table("DetallePedido")]
public class DetallePedido
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdDetalle { get; set; }

    [Range(1, int.MaxValue)]
    public int IdPedido { get; set; }

    [Range(1, int.MaxValue)]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal PrecioUnitario { get; set; }
}
