namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Plugin that does not override ConfigureContext, uses base virtual implementation.
/// </summary>
[PluginId("a1111111-1111-4111-8111-111111111111")]
public class DefaultConfigureContextPlugin : Plugin
{
    public bool InstallCalled { get; private set; }
    public bool ConfigureCalled { get; private set; }
    public Exception? ConfigureException { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallCalled = true;
        // register a simple service so Configure can resolve it if needed
        services.AddSingleton<TestPluginService>();
    }

    // Note: no override for ConfigureContext -> default implementation used

    public override async Task Configure(IServiceProvider container, object? host = null)
    {
        try
        {
            await Task.Delay(1);
            ConfigureCalled = true;
        }
        catch (Exception ex)
        {
            ConfigureException = ex;
            throw;
        }
    }
}