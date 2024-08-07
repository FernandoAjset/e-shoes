using LCDE.Models;

namespace LCDE.Servicios
{
    public interface IRepositotioClientes
    {
        Task<bool> BorrarCliente(int IdCliente);
        Task<int> CrearCliente(Cliente cliente);
        Task<bool> ModificarCliente(Cliente cliente);
        Task<Cliente> ObtenerCliente(int IdCliente);
        Task<Cliente> ObtenerClientePorNit(string NIT, int Id);
        Task<IEnumerable<Cliente>> ObtenerTodosClientes();
    }
}