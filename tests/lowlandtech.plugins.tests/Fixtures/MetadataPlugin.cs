namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class MetadataPlugin : Plugin
{
    public MetadataPlugin()
    {
        Name = "BackendPlugin";
        Description = "Sample backend plugin";
        Company = "LowlandTech";
        Version = "1.0.0";
        IsActive = true;
    }

    public override void Install(IServiceCollection services)
    {
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }
}
