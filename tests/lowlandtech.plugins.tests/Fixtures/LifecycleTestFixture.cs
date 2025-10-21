namespace LowlandTech.Plugins.Tests.Fixtures;

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
/// Test service with transient lifetime.
/// </summary>
public class TransientTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}

// Additional test plugins for scenarios SC07-SC12

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