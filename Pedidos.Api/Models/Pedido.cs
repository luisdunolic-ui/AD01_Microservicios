using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Api.Models;

[Table("Pedido")]
public class Pedido
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdPedido { get; set; }

    [Range(1, int.MaxValue)]
    public int IdCliente { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(12,2)")]
    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal Total { get; set; }

    [Required]
    [RegularExpression("^(Registrado|Procesando|Despachado|Cancelado)$")]
    [StringLength(20)]
    public string Estado { get; set; } = "Registrado";
}
