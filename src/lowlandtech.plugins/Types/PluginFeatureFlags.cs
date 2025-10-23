namespace LowlandTech.Plugins.Types;

/// <summary>
/// Represents a set of feature flags for configuring plugin behavior.
/// </summary>
/// <remarks>This class provides a collection of boolean flags that control various features within a plugin. Each
/// flag can be enabled or disabled to customize the plugin's functionality according to specific
/// requirements.</remarks>
public sealed class PluginFeatureFlags
{
    /// <summary>
    /// Gets or sets a value indicating whether seeding is enabled.
    /// </summary>
    public bool EnableSeeding { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether OpenTelemetry is enabled.
    /// </summary>
    public bool EnableOpenTelemetry { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether health checks are enabled.
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether rate limiting is enabled.
    /// </summary>
    public bool EnableRateLimiting { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether outbound HTTP requests are enabled.
    /// </summary>
    public bool EnableOutboundHttp { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether authentication is required.
    /// </summary>
    public bool RequireAuth { get; set; } = true;
}