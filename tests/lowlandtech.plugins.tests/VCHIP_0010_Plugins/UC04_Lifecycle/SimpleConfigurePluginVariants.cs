using LowlandTech.Plugins.Tests.Fixtures;

namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC04_Lifecycle;

// Distinct subclasses of SimpleConfigurePlugin with unique PluginId attributes
[PluginId("d4444444-4444-4444-8444-444444444441")]
public class SimpleConfigurePluginA : SimpleConfigurePlugin
{
    public bool IsActive { get; set; }
}

[PluginId("d4444444-4444-4444-8444-444444444442")]
public class SimpleConfigurePluginB : SimpleConfigurePlugin { }

[PluginId("d4444444-4444-4444-8444-444444444443")]
public class SimpleConfigurePluginC : SimpleConfigurePlugin { }
