using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using System.Text;

namespace MinhaPrimeiraApi.Services;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseHangfireDashboardCustom(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter() }
        });

        return app;
    }
}

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // 1. Tenta pegar o cabeçalho de autorização
        string header = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrWhiteSpace(header))
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        try
        {
            // 2. O formato do Basic Auth é "Basic base64(user:pass)"
            var authHeader = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);
            
            if ("Basic".Equals(authHeader.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter!)).Split(':');
                var user = credentials[0];
                var password = credentials[1];
                
                var envUser = Environment.GetEnvironmentVariable("HANGFIRE_USER");
                var envPass = Environment.GetEnvironmentVariable("HANGFIRE_PWD");

                // 3. Verifica se é adm / adm
                if (user == envUser && password == envPass)
                {
                    return true;
                }
            }
        }
        catch
        {
            // Se der erro no parse, nega o acesso
        }

        SetChallengeResponse(httpContext);
        return false;
    }

    private void SetChallengeResponse(Microsoft.AspNetCore.Http.HttpContext httpContext)
    {
        // Força o navegador a abrir a janelinha de login (Username/Password)
        httpContext.Response.StatusCode = 401;
        httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
    }
}