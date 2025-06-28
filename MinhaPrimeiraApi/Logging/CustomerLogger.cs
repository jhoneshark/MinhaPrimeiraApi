namespace MinhaPrimeiraApi.Logging;

public class CustomerLogger : ILogger
{
    readonly string loggerName;
    
    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public IDisposable? BeginScope<TState>(TState state)
    {
        return null;
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string mensagem = $"{logLevel.ToString()}: {eventId.Id} {formatter(state, exception)}";

        EscreverTextoNoArquivo(mensagem);
    }
    private void EscreverTextoNoArquivo(string mensagem)
    {
        try
        {
            // Use uma pasta segura no Mac
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Logs");

            // Cria a pasta se não existir
            Directory.CreateDirectory(logDirectory);

            string loggerName = "log"; // Nome fixo para o arquivo
            string caminhoArquivo = Path.Combine(logDirectory, $"{loggerName}.txt");

            using (StreamWriter streamWriter = new StreamWriter(caminhoArquivo, true))
            {
                streamWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {mensagem}");
            }

            Console.WriteLine("Log gravado em: " + caminhoArquivo);
        }
        catch (Exception e)
        {
            Console.WriteLine("--- ERRO AO GRAVAR LOG EM ARQUIVO ---");
            Console.WriteLine(e);
            Console.WriteLine("-------------------------------------");
        }
    }

}