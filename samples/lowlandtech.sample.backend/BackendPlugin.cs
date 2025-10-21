namespace LowlandTech.Sample.Backend;

[PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
public class BackendPlugin : Plugin
{
    public override void Install(IServiceCollection services)
    {
        // services can be registered here
        services.AddSingleton<BackendActivity>();
    }

    public override Task Configure(IServiceProvider provider, object? host = null)
    {
        if (host is null) return Task.CompletedTask;

        var app = (WebApplication) host;
        app.MapGet("/weatherforecast", () =>
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55),
                        summaries[Random.Shared.Next(summaries.Length)]
                    ))
                .ToArray();
            return forecast;
        });

        return Task.CompletedTask;
    }
}