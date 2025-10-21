namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC00_Configuration;

/// <summary>
/// Test fixture for Plugin Configuration tests.
/// Provides common setup for configuration-related test scenarios.
/// </summary>
public sealed class ConfigurationTestFixture
{
    /// <summary>
    /// Creates a test configuration with plugin settings.
    /// </summary>
    public IConfiguration CreateConfiguration(Dictionary<string, string> configData)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }

    /// <summary>
    /// Creates a default plugin configuration for testing.
    /// </summary>
    public IConfiguration CreateDefaultConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            ["Plugins:0:Name"] = "LowlandTech.Sample.Backend",
            ["Plugins:0:IsActive"] = "true"
        };

        return CreateConfiguration(configData);
    }
}
