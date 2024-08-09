using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace LCDE.Servicios
{
    public static class LeerTemplateService

    {

        private static string basePath = Assembly
                                            .GetExecutingAssembly()
                                            .Location
                                            .Replace("LCDE.dll",
                                                        string.Empty);

        // Allows setting the base path for tests

        public static void SetBasePath(string path)

        {

            basePath = path;

        }

        public static string GetTemplateToStringByName(string name)

        {

            if (string.IsNullOrWhiteSpace(name))

            {

                throw new ArgumentException("Template name cannot be null, empty, or whitespace.", nameof(name));

            }

            try

            {

                string templatePath = $"{basePath}Data\\{name}";

                using StreamReader reader = new(templatePath);

                return reader.ReadToEnd();

            }

            catch (FileNotFoundException)

            {

                throw new Exception("No se pudo obtener el archivo del correo.");

            }

            catch (Exception ex)

            {

                throw new Exception("Ocurrió un error al obtener el archivo.");

            }

        }

    }

}
