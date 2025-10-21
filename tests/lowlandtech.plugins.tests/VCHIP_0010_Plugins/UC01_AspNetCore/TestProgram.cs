namespace LowlandTech.Plugins.Tests.VCHIP_0010_Plugins.UC01_AspNetCore;

/// <summary>
/// Minimal test program for ASP.NET Core plugin testing.
/// </summary>
public class TestProgram
{
    public static WebApplication CreateTestApp(Action<WebApplicationBuilder>? configureBuilder = null)
    {
        var builder = WebApplication.CreateBuilder();

        // Allow custom configuration
        configureBuilder?.Invoke(builder);

        var app = builder.Build();
        return app;
    }
}
