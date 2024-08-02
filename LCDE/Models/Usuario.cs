namespace LCDE.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre_usuario { get; set; }
        public string Contrasennia { get; set; }
        public string Correo { get; set; }
        public int Id_Role { get; set; }
        public string Rol { get; set; }

    }
}
