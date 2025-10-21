namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

/// <summary>
/// Test fixture for lifecycle tests.
/// </summary>
public class LifecycleTestFixture
{
    public LifecycleTestFixture()
    {
    }
}

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

/// <summary>
/// Test service registered by plugins during Install phase.
/// </summary>
public class TestPluginService
{
    public string Message { get; } = "Test service registered during Install";
}

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

/// <summary>
/// Test service with singleton lifetime.
/// </summary>
public class SingletonTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

/// <summary>
/// Test service with scoped lifetime.
/// </summary>
public class ScopedTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

/// <summary>
/// Test service with transient lifetime.
/// </summary>
public class TransientTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

// Additional test plugins for scenarios SC07-SC12

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

/// <summary>
/// Plugin that throws during Install to test error handling.
/// </summary>
[PluginId("b2222222-2222-4222-8222-222222222222")]
public class ThrowingInstallPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        throw new InvalidOperationException("Install failed");
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}

/// <summary>
/// Plugin that throws during ConfigureContext to test error handling.
/// </summary>
[PluginId("c3333333-3333-4333-8333-333333333333")]
public class ThrowingConfigureContextPlugin : Plugin
{
    public bool ConfigureCalled { get; private set; }

    public override void Install(IServiceCollection services)
    {
        // normal install
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        throw new InvalidOperationException("ConfigureContext failed");
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ConfigureCalled = true;
        return Task.CompletedTask;
    }
}

/// <summary>
/// Simple plugin that records Configure being called (used by SC08).
/// </summary>
[PluginId("d4444444-4444-4444-8444-444444444444")]
public class SimpleConfigurePlugin : Plugin
{
    public bool InstallCalled { get; private set; }
    public bool ConfigureCalled { get; private set; }
    public IServiceProvider? ReceivedProvider { get; private set; }
    public object? ReceivedHost { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallCalled = true;
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        ReceivedProvider = container;
        ReceivedHost = host;
        ConfigureCalled = true;
        return Task.CompletedTask;
    }
}
