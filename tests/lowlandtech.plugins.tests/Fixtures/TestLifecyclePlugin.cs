namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Test plugin that tracks lifecycle method calls.
/// </summary>
[PluginId("e8f7d6c5-b4a3-4291-8f7e-6d5c4b3a2918")]
public class TestLifecyclePlugin : Plugin
{
    public bool InstallCalled { get; set; }
    public bool ConfigureContextCalled { get; set; }
    public bool ConfigureCalled { get; set; }
    
    public Exception? InstallException { get; set; }
    public Exception? ConfigureContextException { get; set; }
    public Exception? ConfigureException { get; set; }
    
    public IServiceProvider? ServiceProviderReceived { get; set; }
    public object? HostReceived { get; set; }
    
    public bool RouteConfigured { get; set; }
    public bool MiddlewareConfigured { get; set; }
    
    public Action<string>? OnPhaseExecuted { get; set; }

    public override void Install(IServiceCollection services)
    {
        try
        {
            InstallCalled = true;
            OnPhaseExecuted?.Invoke("Install");
            
            // Register a test service
            services.AddSingleton<TestPluginService>();
        }
        catch (Exception ex)
        {
            InstallException = ex;
            throw;
        }
    }

    public override async Task ConfigureContext(IServiceCollection services)
    {
        try
        {
            await Task.Delay(1); // Simulate async work
            ConfigureContextCalled = true;
            OnPhaseExecuted?.Invoke("ConfigureContext");
        }
        catch (Exception ex)
        {
            ConfigureContextException = ex;
            throw;
        }
    }

    public override async Task Configure(IServiceProvider container, object? host = null)
    {
        try
        {
            await Task.Delay(1); // Simulate async work
            ConfigureCalled = true;
            ServiceProviderReceived = container;
            HostReceived = host;
            OnPhaseExecuted?.Invoke("Configure");
            
            // If host is WebApplication, configure routes and middleware
            if (host is WebApplication app)
            {
                app.MapGet("/test-lifecycle-plugin", () => "Test Lifecycle Plugin");
                RouteConfigured = true;
                
                app.Use(async (context, next) =>
                {
                    await next();
                });
                MiddlewareConfigured = true;
            }
        }
        catch (Exception ex)
        {
            ConfigureException = ex;
            throw;
        }
    }
}