using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LCDE.Servicios
{
    public interface IFileRepository
    {
        public Task<string> AddFile(IFormFile file, string contenedor);
        public Task DeleteFile(string ruta, string contenedor);
    }

    public class AzureFileRepository : IFileRepository
    {
        private readonly string AzureConnection;

        public AzureFileRepository(IConfiguration configuration)
        {
            AzureConnection = configuration.GetConnectionString("AzureStorage");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="File"></param>
        /// Es el archivo que se va a subir
        ///
        /// <param name="contenedor"></param>
        /// Es el nombre de la carpeta donde se va a guardar el archivo
        /// 
        /// <returns>Retorna la ruta en la que se subio el archivo</returns>
        public async Task<string> AddFile(IFormFile File, string contenedor)
        {

            byte[] contenido = new byte[0];
            string extension = "";
            using (var memoryStream = new MemoryStream())
            {
                await File.CopyToAsync(memoryStream);
                contenido = memoryStream.ToArray();
                extension = Path.GetExtension(File.FileName);
            }

            var cliente = new BlobContainerClient(AzureConnection, contenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var archivoNombre = $"{Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(archivoNombre);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = File.ContentType;
            blobUploadOptions.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);
            return blob.Uri.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruta"></param>
        /// Ruta en donde se encuentra el archivo
        /// 
        /// <param name="contenedor"></param>
        /// Es el nombre de la carpeta donde se va a guardar el archivo
        /// 
        /// <returns></returns>
        public async Task DeleteFile(string ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(AzureConnection, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var archivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(archivo);
            await blob.DeleteIfExistsAsync();
        }
    }

}
