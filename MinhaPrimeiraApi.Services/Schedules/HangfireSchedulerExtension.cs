using Microsoft.AspNetCore.Builder;
using Hangfire;

namespace MinhaPrimeiraApi.Services.Schedules;

// Adicione 'static' aqui para o método de extensão funcionar
public static class HangfireSchedulerExtension
{
    public static void RegisterHangfireJobs(this IApplicationBuilder app)
    {
        // Agendamento de teste: Roda a cada minuto (Cron.Minutely)
        RecurringJob.AddOrUpdate<TesteService>(
            "job-teste-console", 
            x => x.EscreverNoConsole(), 
            Cron.Minutely
        );
    }
}