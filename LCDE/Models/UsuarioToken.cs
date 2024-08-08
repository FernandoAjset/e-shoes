namespace LCDE.Models
{
    public class UsuarioToken
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Token {  get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Fecha_Solicitud { get; set; }
        public DateTime Fecha_Vencimiento { get; set; }
        public bool Activo {  get; set; }
    }
}
