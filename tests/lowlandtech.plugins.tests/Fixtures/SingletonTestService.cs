namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Test service with singleton lifetime.
/// </summary>
public class SingletonTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}