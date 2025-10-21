namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

// Plugin that throws during Configure phase
[PluginId("e5555555-5555-4555-8555-555555555555")]
public class ThrowingConfigurePlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // normal install
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        throw new InvalidOperationException("Configure failed");
    }
}

// Plugin with metadata set to verify preservation
[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class MetadataPlugin : Plugin
{
    public MetadataPlugin()
    {
        // set metadata
        Name = "BackendPlugin";
        IsActive = true;
        // use protected setters where available
        var type = this.GetType();
        // Use reflection to set protected properties
        type.GetProperty("Description", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?.SetValue(this, "Sample backend plugin");
        type.GetProperty("Company", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?.SetValue(this, "LowlandTech");
        type.GetProperty("Version", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?.SetValue(this, "1.0.0");
    }

    public override void Install(IServiceCollection services)
    {
        // no-op
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        return Task.CompletedTask;
    }
}

// Simple plugin used to test independent lifecycle execution
[PluginId("f6666666-6666-4666-8666-666666666666")]
public class IndependentPlugin : Plugin
{
    public bool InstallStarted { get; private set; }
    public bool InstallCompleted { get; private set; }

    public override void Install(IServiceCollection services)
    {
        InstallStarted = true;
        // simulate some work but not throw
        Thread.Sleep(10);
        InstallCompleted = true;
    }

    public override Task ConfigureContext(IServiceCollection services) => Task.CompletedTask;
    public override Task Configure(IServiceProvider container, object? host = null) => Task.CompletedTask;
}

// Plugin that checks Assemblies collection availability
[PluginId("a7777777-7777-4777-8777-777777777777")]
public class AssembliesAwarePlugin : Plugin
{
    public bool AssembliesAvailableInInstall { get; private set; }
    public bool AssembliesAvailableInConfigureContext { get; private set; }
    public bool AssembliesAvailableInConfigure { get; private set; }

    public override void Install(IServiceCollection services)
    {
        AssembliesAvailableInInstall = Assemblies is not null && Assemblies.Count > 0;
    }

    public override Task ConfigureContext(IServiceCollection services)
    {
        AssembliesAvailableInConfigureContext = Assemblies is not null && Assemblies.Count > 0;
        return Task.CompletedTask;
    }

    public override Task Configure(IServiceProvider container, object? host = null)
    {
        AssembliesAvailableInConfigure = Assemblies is not null && Assemblies.Count > 0;
        return Task.CompletedTask;
    }
}
