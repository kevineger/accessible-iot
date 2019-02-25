using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;

public interface IModule
{
    void Load(IServiceCollection services);
}