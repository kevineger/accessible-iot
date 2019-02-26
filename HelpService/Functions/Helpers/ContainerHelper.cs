using System;
using Microsoft.Azure.WebJobs;

public static class ContainerHelper
{
    public static IServiceProvider Build(ExecutionContext context)
    {
        return new ContainerBuilder()
                .RegisterModule(new Module(context))
                .Build();
    }
}