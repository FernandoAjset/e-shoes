using System.Text;

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

            var valueHash = BCrypt.Net.BCrypt.HashPassword(value, workFactor: 13);

            // Convertir el hash a bytes
            var hashBytes = Encoding.UTF8.GetBytes(valueHash);

            // Codificar en Base64 URL-safe
            var base64UrlSafeHash = Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", string.Empty);

            return base64UrlSafeHash;
        }
    }
}
