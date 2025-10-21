namespace LowlandTech.Sample.Frontend;

[PluginId("4a8c6f2e-1b3d-4e5f-9a7c-8d6e5f4a3b2c")]
public class FrontendPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<FrontendService>();
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }
}

public class FrontendService { }
