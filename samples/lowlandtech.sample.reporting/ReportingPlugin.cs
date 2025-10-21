namespace LowlandTech.Sample.Reporting;

[PluginId("e1c3a2d4-5f6b-47d8-9a1b-2c3d4e5f6a7b")]
public class ReportingPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<ReportingService>();
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}

public class ReportingService { }
