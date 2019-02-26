using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Module : IModule
{
    private readonly ExecutionContext context;

    public Module(ExecutionContext context)
    {
        this.context = context;
    }

    public void Load(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddOptions();
        services.Configure<AzureTableOptions>(options => configuration.GetSection("AzureTables").Bind(options));
        services.Configure<AzureMapsOptions>(options => configuration.GetSection("AzureMaps").Bind(options));
        services.AddSingleton<ICurrentLocationRepository, CurrentLocationRepository>();
        services.AddSingleton<IAcknoledgeRepository, AcknoledgeRepository>();
        services.AddSingleton<IAzureMapsRepository, AzureMapsRepository>();
    }
}