namespace LCDE.Servicios
{
    public interface IEncryptService
    {
        string HashString(string value);
    }
    public class EncryptService : IEncryptService
    {
        public string HashString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Valor no puede ser vacío", nameof(value));
            }

            var ValueHash = BCrypt.Net.BCrypt.HashPassword(value, workFactor: 13);
            return ValueHash;
        }
    }
}
