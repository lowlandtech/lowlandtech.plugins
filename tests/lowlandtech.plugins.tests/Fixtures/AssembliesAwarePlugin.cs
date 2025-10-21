using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowlandTech.Plugins;

namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("c2222222-0000-4000-8000-000000000022")]
public class AssembliesAwarePlugin : Plugin
{
    public bool AssembliesAvailableInInstall { get; private set; }
    public bool AssembliesAvailableInConfigureContext { get; private set; }
    public bool AssembliesAvailableInConfigure { get; private set; }

    public override void Install(IServiceCollection services)
    {
        AssembliesAvailableInInstall = Assemblies is not null && Assemblies.Any();
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        AssembliesAvailableInConfigureContext = Assemblies is not null && Assemblies.Any();
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        AssembliesAvailableInConfigure = Assemblies is not null && Assemblies.Any();
        return Task.CompletedTask;
    }
}
