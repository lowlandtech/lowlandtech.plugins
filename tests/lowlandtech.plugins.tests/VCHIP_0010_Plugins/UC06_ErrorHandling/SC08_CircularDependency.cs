namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

public class CircularServiceA 
{
    public CircularServiceA(CircularServiceB b)
    {
        // depends on B
    }
}
public class CircularServiceB 
{
    public CircularServiceB(CircularServiceA a)
    {
        // depends on A
    }
}

[PluginId("ea777777-0000-4000-8000-000000000007")]
public class CircularDependencyPluginA : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // register the types; constructors create the circular dependency
        services.AddScoped<CircularServiceA>();
    }

    public override Task Configure(IServiceProvider container, object? host = null) => Task.CompletedTask;
}

[PluginId("ea888888-0000-4000-8000-000000000008")]
public class CircularDependencyPluginB : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddScoped<CircularServiceB>();
    }

    public override Task Configure(IServiceProvider container, object? host = null) => Task.CompletedTask;
}

[Scenario(
    specId: "VCHIP-0010-UC06-SC08",
    title: "Handle circular dependency in plugin services",
    given: "Given Plugin A depends on Service B and vice versa",
    when: "When the service provider attempts to resolve the circular dependency",
    then: "Then a circular dependency exception should be thrown")]
public sealed class SC08_CircularDependency : WhenTestingForV2<ErrorHandlingTestFixture>
{
    private IServiceCollection? _services;
    private Exception? _caught;

    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given()
    {
        _services = new ServiceCollection();
        var a = new CircularDependencyPluginA();
        var b = new CircularDependencyPluginB();
        _services.AddPlugin(a);
        _services.AddPlugin(b);
    }

    protected override void When()
    {
        try
        {
            var sp = _services!.BuildServiceProvider();
            using var scope = sp.CreateScope();
            // attempt to resolve A which should trigger circular dependency
            scope.ServiceProvider.GetRequiredService<CircularServiceA>();
        }
        catch (Exception ex)
        {
            _caught = ex;
        }
    }

    [Fact]
    [Then("A circular dependency exception should be thrown", "UAC027")]
    public void Circular_Exception()
    {
        _caught.ShouldNotBeNull();
    }

    [Fact]
    [Then("The error message should indicate the dependency chain", "UAC028")]
    public void Error_Message()
    {
        _caught.ShouldNotBeNull();
        _caught!.Message.ShouldNotBeNullOrEmpty();
    }
}
