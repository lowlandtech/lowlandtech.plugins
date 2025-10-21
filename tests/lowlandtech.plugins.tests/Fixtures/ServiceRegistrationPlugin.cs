namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Plugin that registers different service lifetimes.
/// </summary>
[PluginId("f9e8d7c6-b5a4-4392-9f8e-7e6d5c4b3a29")]
public class ServiceRegistrationPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<SingletonTestService>();
        services.AddScoped<ScopedTestService>();
        services.AddTransient<TransientTestService>();
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}