using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("b1111111-0000-4000-8000-000000000011")]
public class IndependentPlugin : Plugin
{
    public bool InstallStarted { get; private set; }
    public bool InstallCompleted { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallStarted = true;
        // Simulate some work
        InstallCompleted = true;
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}
