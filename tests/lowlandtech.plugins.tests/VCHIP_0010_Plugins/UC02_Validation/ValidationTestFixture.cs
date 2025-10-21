namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC02_Validation;

public class ValidationTestFixture
{
}

[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class ValidPlugin : Plugin
{
    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }

    public override void Install(IServiceCollection services)
    {
    }
}

public class InvalidPluginNoId : Plugin
{
    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        return Task.CompletedTask;
    }

    public override void Install(IServiceCollection services)
    {
    }
}
