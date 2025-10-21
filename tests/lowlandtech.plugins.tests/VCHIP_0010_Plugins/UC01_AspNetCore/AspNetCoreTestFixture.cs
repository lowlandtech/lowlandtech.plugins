namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

/// <summary>
/// Test fixture for ASP.NET Core integration tests.
/// Provides common setup and utilities for testing plugin integration with ASP.NET Core.
/// </summary>
public sealed class AspNetCoreTestFixture
{
    /// <summary>
    /// Creates a WebApplicationBuilder with test configuration.
    /// </summary>
    public WebApplicationBuilder CreateBuilder(Action<IServiceCollection>? configureServices = null)
    {
        var builder = WebApplication.CreateBuilder();

        configureServices?.Invoke(builder.Services);

        return builder;
    }

    /// <summary>
    /// Creates a test WebApplication with a plugin registered.
    /// </summary>
    public WebApplication CreateAppWithPlugin(IPlugin plugin)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddPlugin(plugin);

        var app = builder.Build();
        app.UsePlugins();

        return app;
    }

    /// <summary>
    /// Creates a test configuration with plugin settings.
    /// </summary>
    public IConfiguration CreateConfiguration(Dictionary<string, string> configData)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();
    }
}
