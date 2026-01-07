namespace MinhaPrimeiraApi.Services;

public class TesteService
{
    public void EscreverNoConsole()
    {
        Console.WriteLine($"[Hangfire Job] Log de teste executado em: {DateTime.Now}");
    }
}