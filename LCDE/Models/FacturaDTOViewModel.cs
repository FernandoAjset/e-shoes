using LCDE.Models.Enums;

namespace LCDE.Models
{
    public class VistaFacturaDTO
    {
        public int Id_Factura { get; set; } // Correspondiente a ef.id
        public string Serie { get; set; } // Correspondiente a ef.serie
        public DateTime Fecha { get; set; } // Correspondiente a ef.fecha
        public FacturaEstadoEnum estado_factura_id { get; set; } // Correspondiente a ef.estado_factura_id

        public string Nombre_cliente { get; set; } // Correspondiente a c.nombre
        public string Direccion { get; set; } // Correspondiente a c.direccion
        public string Telefono { get; set; } // Correspondiente a c.telefono
        public string Correo { get; set; } // Correspondiente a c.correo
        public string NIT { get; set; } // Correspondiente a c.NIT

        public decimal Subtotal { get; set; } // Correspondiente a df.subtotal
        public string Url { get; set; } // Correspondiente a ef.url_pago

        public int Cantidad { get; set; } // Correspondiente a df.cantidad
        public string nombre_producto { get; set; } // Correspondiente a p.nombre
        public string descripcion_producto { get; set; } // Correspondiente a p.detalle
        public decimal precio_unidad { get; set; } // Correspondiente a pp.precio_unidad
        public string Categoria { get; set; }
        public string ImageUrl { get; set; } // Correspondiente a p.image_url
    }


    public class FacturaDTOViewModel
    {
        public int IdFactura { get; set; }
        public string Serie { get; set; }
        public DateTime Fecha { get; set; }
        public FacturaEstadoEnum Estado { get; set; }
        public ClienteDTOViewModel Cliente { get; set; }
        public decimal Subtotal { get; set; }
        public string Url { get; set; }
        public List<DetalleFacturaDTOViewModel> Detalles { get; set; }
    }

    public class ClienteDTOViewModel
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string NIT { get; set; }
    }

    public class DetalleFacturaDTOViewModel
    {
        public int Cantidad { get; set; }
        public string NombreProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal PrecioUnidad { get; set; }
        public decimal Subtotal { get; set; }
        public string Categoria { get; set; }
        public string ImageUrl { get; set; }
    }

}
