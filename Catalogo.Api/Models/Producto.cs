using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalogo.Api.Models;

[Table("Producto")]
public class Producto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue)]
    public int IdCategoria { get; set; }

    [Range(1, int.MaxValue)]
    public int IdMarca { get; set; }

    [Required]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0", "99999999.99")]
    public decimal Precio { get; set; }

    public bool Estado { get; set; } = true;
}
