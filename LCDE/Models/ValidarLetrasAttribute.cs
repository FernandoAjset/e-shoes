using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class ValidarLetrasAttribute : ValidationAttribute
{
    public ValidarLetrasAttribute()
    {
        // Mensaje de error predeterminado
        ErrorMessage = "El campo solo puede contener letras de la A a la Z.";
    }

    // Método que verifica la validez del valor
    public override bool IsValid(object value)
    {
        if (value == null)
        {
            return true; // Manejar valores nulos con otra validación (por ejemplo, [Required])
        }

        // Convertir el valor a cadena
        string valorCadena = value as string;

        if (string.IsNullOrEmpty(valorCadena))
        {
            return true; // Manejar cadenas vacías con otra validación
        }

        // Expresión regular para solo letras
        Regex regex = new Regex(@"^[a-zA-Z\s]+$");
        return regex.IsMatch(valorCadena);
    }
}