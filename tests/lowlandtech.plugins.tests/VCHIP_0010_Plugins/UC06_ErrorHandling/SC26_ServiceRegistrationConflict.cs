namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC06_ErrorHandling;

public interface ITestService { }
public class TestServiceImplA : ITestService { }
public class TestServiceImplB : ITestService { }

[PluginId("f1111111-0000-4000-8000-000000000001")]
public class ServiceConflictPluginA : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<ITestService, TestServiceImplA>();
    }
    public override Task Configure(IServiceProvider container, object? host = null) => Task.CompletedTask;
}

[PluginId("f2222222-0000-4000-8000-000000000002")]
public class ServiceConflictPluginB : Plugin
{
    public override void Install(IServiceCollection services)
    {
        services.AddSingleton<ITestService, TestServiceImplB>();
    }
    public override Task Configure(IServiceProvider container, object? host = null) => Task.CompletedTask;
}

[Scenario(
    specId: "VCHIP-0010-UC06-SC26",
    title: "Handle plugin service registration conflict",
    given: "Given Plugin A registers IService with ImplementationA and Plugin B registers IService with ImplementationB",
    when: "When both plugins install their services",
    then: "Then the behavior depends on DI container rules")]
public sealed class SC26_ServiceRegistrationConflict : WhenTestingForV2<ErrorHandlingTestFixture>
{
    protected override ErrorHandlingTestFixture For() => new();

    protected override void Given() { }
    protected override void When() { }

    [Fact]
    [Then("The behavior depends on DI container rules (last registration wins)", "UAC080")]
    public void Last_Registration_Wins()
    {
        var services = new ServiceCollection();
        services.AddPlugin(new ServiceConflictPluginA());
        services.AddPlugin(new ServiceConflictPluginB());
        
        var sp = services.BuildServiceProvider();
        var service = sp.GetRequiredService<ITestService>();
        
        // Last registration wins in default DI container
        service.ShouldBeOfType<TestServiceImplB>();
    }

    [Fact]
    [Then("Developers should be aware of service registration order", "UAC082")]
    public void Registration_Order_Matters()
    {
        // Document that plugin registration order affects which implementation is used
        // when multiple plugins register the same interface
        var services = new ServiceCollection();
        services.AddPlugin(new ServiceConflictPluginB());
        services.AddPlugin(new ServiceConflictPluginA());
        
        var sp = services.BuildServiceProvider();
        var service = sp.GetRequiredService<ITestService>();
        
        // Order reversed: A is last, so A wins
        service.ShouldBeOfType<TestServiceImplA>();
    }
}
