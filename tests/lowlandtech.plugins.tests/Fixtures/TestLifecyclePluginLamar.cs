using Lamar;

namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Lamar-compatible test plugin that tracks lifecycle method calls and does not depend on ASP.NET Core types.
/// </summary>
[PluginId("d1a2f3e4-5678-4a2b-9cde-1234567890ab")]
public class TestLifecyclePluginLamar : Plugin
{
    public bool InstallCalled { get; set; }
    public bool ConfigureContextCalled { get; set; }
    public bool ConfigureCalled { get; set; }

    public Exception? InstallException { get; set; }
    public Exception? ConfigureContextException { get; set; }
    public Exception? ConfigureException { get; set; }

    public IServiceProvider? ServiceProviderReceived { get; set; }
    public object? HostReceived { get; set; }

    public Action<string>? OnPhaseExecuted { get; set; }

    // Existing IServiceCollection-based install kept for compatibility
    public override void Install(IServiceCollection services)
    {
        try
        {
            InstallCalled = true;
            OnPhaseExecuted?.Invoke("Install");
            // Register a simple service to validate DI registration
            services.AddSingleton<TestPluginService>();
        }
        catch (Exception ex)
        {
            InstallException = ex;
            throw;
        }
    }

    // Lamar-specific override to support ServiceRegistry being passed in
    public override void Install(ServiceRegistry services)
    {
        // Delegate to the IServiceCollection overload to keep behavior consistent
        InstallCalled = true;
        OnPhaseExecuted?.Invoke("Install");
        try
        {
            Install((IServiceCollection)services);
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
            await Task.Delay(1);
            ConfigureContextCalled = true;
            OnPhaseExecuted?.Invoke("ConfigureContext");
        }
        catch (Exception ex)
        {
            ConfigureContextException = ex;
            throw;
        }
    }

    // Existing IServiceProvider-based configure kept for compatibility
    public override async Task Configure(IServiceProvider container, object? host = null)
    {
        try
        {
            await Task.Delay(1);
            ConfigureCalled = true;
            ServiceProviderReceived = container;
            HostReceived = host;
            OnPhaseExecuted?.Invoke("Configure");
            // Lamar environment: do not assume WebApplication; just ensure container is usable
            var svc = container.GetService<TestPluginService>();
        }
        catch (Exception ex)
        {
            ConfigureException = ex;
            throw;
        }
    }

    // Lamar-specific override to support IContainer being passed in
    public override async Task Configure(IContainer container, object? host = null)
    {
        try
        {
            // Delegate to the IServiceProvider-based implementation for consistent behavior
            await Configure((IServiceProvider)container, host);
        }
        catch (Exception ex)
        {
            ConfigureException = ex;
            throw;
        }
    }
}
