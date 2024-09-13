namespace LCDE.Models
{
    public class EncabezadoFactura
    {
        public int Id { get; set; }
        public string Serie { get; set; }
        public DateTime? Fecha { get; set; }
        public int IdTipoPago { get; set; }
        public int IdCliente { get; set; }
        public int EstadoFacturaId { get; set; }
    }
}
