using System;
using Microsoft.Extensions.DependencyInjection;

public interface IContainerBuilder
{
    IContainerBuilder RegisterModule(IModule module);

    IServiceProvider Build();
}

public class ContainerBuilder : IContainerBuilder
{
    private readonly IServiceCollection services;

    public ContainerBuilder()
    {
        this.services = new ServiceCollection();
    }

    public IContainerBuilder RegisterModule(IModule module)
    {
        module.Load(this.services);

        return this;
    }

    public IServiceProvider Build()
    {
        var provider = this.services.BuildServiceProvider();

        return provider;
    }
}