namespace LCDE.Models
{
    public class PaypalDTO
    {
        public string Precio { get; set; }
        public string Descripcion { get; set; }
        public int IdOrden { get; set; }
    }

    public class PaypalLink
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public class PaypalOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<PaypalLink> links { get; set; }
    }
}