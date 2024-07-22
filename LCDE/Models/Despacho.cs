namespace LCDE.Models
{
   
    public class Despacho
        {
            public int Id { get; set; }
            public int IdEncabezadoFactura { get; set; }
            public DateTime FechaDespacho { get; set; }
            public string Estado { get; set; }
        }
    
}
