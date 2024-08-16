using LCDE.Models.Enums;
using System.Security.Claims;

public class RoleBasedRedirectionMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedRedirectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true && context.Request.Path == "/")
        {
            var userRole = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userRole))
            {
                if (userRole == ((int)Rol.Cliente).ToString())
                {
                    context.Response.Redirect("/Ecommerce/Home");
                    return;
                }
                else if (userRole == ((int)Rol.Admin).ToString() || userRole == ((int)Rol.Vendedor).ToString())
                {
                    context.Response.Redirect("/Ventas/Index");
                    return;
                }
            }
        }

        await _next(context);
    }
}