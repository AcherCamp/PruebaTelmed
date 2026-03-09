using System.Security.Claims;

public class ForcePasswordChangeMiddleware
{
    private readonly RequestDelegate _next;

    public ForcePasswordChangeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            var mustChange = user.Claims
                .FirstOrDefault(c => c.Type == "MustChangePassword");

            if (mustChange != null && mustChange.Value == "true")
            {
                var path = context.Request.Path.ToString().ToLower();

                // Permitimos solo login y change-password
                // Solo permitimos estas rutas
                if (path != "/api/auth/change-password" &&
                    path != "/api/auth/login")
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(
                        "Debe cambiar su contraseña antes de acceder al sistema."
                    );
                    return;
                }
            }
        }

        await _next(context);
    }
}