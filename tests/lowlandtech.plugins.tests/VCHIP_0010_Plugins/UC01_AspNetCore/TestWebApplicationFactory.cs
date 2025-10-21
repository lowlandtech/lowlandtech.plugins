namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

/// <summary>
/// Test fixture for ASP.NET Core integration tests.
/// Provides a WebApplicationFactory for testing plugin integration.
/// </summary>
public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly Action<IServiceCollection>? _configureServices;
    private readonly Action<IConfigurationBuilder>? _configureConfiguration;

    public TestWebApplicationFactory(
        Action<IServiceCollection>? configureServices = null,
        Action<IConfigurationBuilder>? configureConfiguration = null)
    {
        _configureServices = configureServices;
        _configureConfiguration = configureConfiguration;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_configureConfiguration != null)
        {
            builder.ConfigureAppConfiguration(_configureConfiguration);
        }

        if (_configureServices != null)
        {
            builder.ConfigureServices(_configureServices);
        }
    }
}
