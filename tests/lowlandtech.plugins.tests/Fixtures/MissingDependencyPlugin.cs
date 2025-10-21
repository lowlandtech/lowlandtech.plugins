namespace LowlandTech.Plugins.Tests.Fixtures;

[PluginId("ea111111-0000-4000-8000-000000000001")]
public class MissingDependencyPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // Attempt to use a type from a missing dependency - simulate by throwing FileNotFoundException during Install
        throw new FileNotFoundException("Missing dependency X") ;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}