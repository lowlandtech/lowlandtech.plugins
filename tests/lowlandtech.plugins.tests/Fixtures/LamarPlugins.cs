using Lamar;

namespace LowlandTech.Plugins.Tests.Fixtures;

public interface ILamarService
{
    string GetMessage();
}

public class LamarService : ILamarService
{
    public string GetMessage() => "Lamar service";
}

[PluginId("a1111111-1111-4111-8111-111111111111")]
public class LamarRegisteringPlugin : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        // Register ILamarService using Lamar-specific syntax only
        services.For<ILamarService>().Use<LamarService>();
    }
}

public interface ILamarAdvancedService
{
    string GetMessage();
}

public class LamarAdvancedService : ILamarAdvancedService
{
    public string GetMessage() => "Lamar advanced service";
}

[PluginId("a2222222-2222-4222-8222-222222222222")]
public class LamarAdvancedPlugin : Plugin
{
    public override void Install(ServiceRegistry services)
    {
        // Simple registration for advanced service; tests only assert message content
        services.For<ILamarAdvancedService>().Use<LamarAdvancedService>();

        // Optionally add decorator or policy examples (omitted for simplicity)
    }
}
