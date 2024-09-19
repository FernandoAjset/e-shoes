namespace LCDE.Models
{
    public class ConfirmarOrdenDTO
    {
        public Cliente ClienteInfo { get; set; }
        public List<CarritoItemDTO> DetallesCarrito { get; set; }
        public string? Observaciones { get; set; }
    }
}