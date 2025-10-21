namespace LowlandTech.Plugins.Tests.Fixtures;

/// <summary>
/// Test service with scoped lifetime.
/// </summary>
public class ScopedTestService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}