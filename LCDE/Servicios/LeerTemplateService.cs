namespace LCDE.Servicios
{
    public class LeerTemplateService : ILeerTemplateService
    {
        private readonly IWebHostEnvironment _env;
        private string basePath = "";
        public LeerTemplateService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GetTemplateToStringByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Template name cannot be null, empty, or whitespace.", nameof(name));
            }

            try
            {
                basePath = Path.Combine(_env.ContentRootPath, "Data");
                string templatePath = Path.Combine(basePath, name);

                using StreamReader reader = new(templatePath);
                return reader.ReadToEnd();
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception($"No se pudo obtener el archivo del correo. Path = {basePath}. Error = {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener el archivo.", ex);
            }
        }
    }

    public interface ILeerTemplateService
    {
        string GetTemplateToStringByName(string name);
    }
}
