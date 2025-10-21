# --------------------------------------------------------------------
# Feature: ASP.NET Core Integration
# --------------------------------------------------------------------

@VCHIP-0010 @UC01 @Plugin @AspNetCore
Feature: ASP.NET Core Integration
  As a developer building an ASP.NET Core application
  I want to integrate plugins seamlessly with ASP.NET Core
  So that plugins can extend my web application with routes, middleware, and services

  Background:
    Given an ASP.NET Core application is being configured
    And the LowlandTech.Plugins.AspNetCore package is referenced

  @SC01
  Scenario: Add plugins to ASP.NET Core service collection
    Given a WebApplicationBuilder is initialized
    When I call builder.Services.AddPlugins()
    Then plugins from configuration should be loaded                                   # UAC001
    And plugins should be registered in the service collection                         # UAC002
    And plugins should be available through dependency injection                       # UAC003

  @SC02 @DirectRegistration
  Scenario: Add a specific plugin by type to service collection
    Given a WebApplicationBuilder is initialized
    And I have a BackendPlugin type
    When I call builder.Services.AddPlugin<BackendPlugin>()
    Then an instance of BackendPlugin should be created                                # UAC004
    And the plugin should be registered in the service collection                      # UAC005
    And the plugin Install method should be called                                     # UAC006

  @SC03 @DirectRegistration
  Scenario: Add a plugin instance to service collection
    Given a WebApplicationBuilder is initialized
    And I have a BackendPlugin instance
    When I call builder.Services.AddPlugin(pluginInstance)
    Then the plugin instance should be registered directly                             # UAC007
    And the plugin Install method should be called                                     # UAC008
    And the plugin should be available for dependency injection                        # UAC009

  @SC04
  Scenario: Use plugins with WebApplication host
    Given plugins have been registered in the service collection
    And the WebApplication has been built
    When I call app.UsePlugins()
    Then the Configure method should be called on each plugin                          # UAC010
    And each plugin should receive the service provider                                # UAC011
    And each plugin should receive the WebApplication as the host parameter            # UAC012

  @SC05
  Scenario: Plugin registers routes during Configure phase
    Given a BackendPlugin is registered
    And the WebApplication is built
    When app.UsePlugins() is called
    Then the plugin should register routes using app.MapGet, app.MapPost, etc.         # UAC013
    And the routes should be accessible in the application                             # UAC014
    And requests to plugin routes should be handled correctly                          # UAC015

  @SC06
  Scenario: Plugin configures middleware during Configure phase
    Given a plugin needs to add middleware
    And the WebApplication is built
    When the plugin Configure method executes
    Then the plugin should be able to call app.UseMiddleware                           # UAC016
    And the middleware should be added to the pipeline                                 # UAC017
    And the middleware should execute for incoming requests                            # UAC018

  @SC07
  Scenario: Multiple plugins register routes without conflicts
    Given multiple plugins are registered:
      | Plugin          | Route Pattern        |
      | BackendPlugin   | /weatherforecast     |
      | FrontendPlugin  | /home                |
      | ApiPlugin       | /api/data            |
    When app.UsePlugins() is called
    Then all routes should be registered successfully                                  # UAC019
    And there should be no route conflicts                                             # UAC020
    And each route should be handled by its respective plugin                          # UAC021

  @SC08
  Scenario: Plugin accesses ASP.NET Core services during Configure
    Given a plugin is registered in an ASP.NET Core application
    And the application has registered ILogger, IConfiguration, etc.
    When the plugin Configure method executes
    Then the plugin should be able to resolve ILogger from the service provider        # UAC022
    And the plugin should be able to resolve IConfiguration                            # UAC023
    And the plugin should be able to use these services for configuration              # UAC024

  @SC09
  Scenario: Plugin with dependency injection in Configure
    Given a BackendPlugin registers a custom service during Install
    And the service is registered as scoped
    When the plugin Configure method executes
    Then the plugin should create a scope to resolve scoped services                   # UAC025
    And the scoped service should be accessible                                        # UAC026
    And the scope should be properly disposed                                          # UAC027

  @SC10
  Scenario: UsePlugins creates scope for service resolution
    Given plugins require scoped services
    When app.UsePlugins() is called
    Then a service scope should be created                                             # UAC028
    And plugins should be resolved from the scoped service provider                    # UAC029
    And the scope should be managed properly to avoid memory leaks                     # UAC030

  @SC11 @OpenAPI
  Scenario: Plugin integrates with ASP.NET Core OpenAPI
    Given an ASP.NET Core application with OpenAPI support
    And a plugin registers API endpoints
    When the plugin routes are registered
    Then the plugin endpoints should appear in the OpenAPI specification               # UAC031
    And the endpoints should be documented correctly                                   # UAC032
    And Swagger UI should show the plugin endpoints                                    # UAC033

  @SC12
  Scenario: Plugin registers endpoint with route attributes
    Given a BackendPlugin registers a /weatherforecast endpoint
    When the endpoint is registered using app.MapGet
    Then the endpoint should respond to GET requests                                   # UAC034
    And the endpoint should return the expected data                                   # UAC035
    And the endpoint should be accessible via HTTP                                     # UAC036

  @SC13
  Scenario: Sample BackendPlugin weatherforecast endpoint
    Given the BackendPlugin is registered
    And the plugin registers the /weatherforecast route
    When a GET request is made to /weatherforecast
    Then the response should contain an array of WeatherForecast objects               # UAC037
    And each forecast should have Date, TemperatureC, and Summary properties           # UAC038
    And the response should return 5 forecast items                                    # UAC039
    And the temperature should be between -20 and 55 degrees                           # UAC040

  @SC14
  Scenario: Plugin configuration in Development vs Production environment
    Given the application environment is set to Development
    And a plugin checks app.Environment.IsDevelopment()
    When the plugin Configure method executes
    Then the plugin should enable development-specific features                        # UAC041
    And the plugin should register development endpoints                               # UAC042

  @SC15
  Scenario: Plugin configuration in Production environment
    Given the application environment is set to Production
    And a plugin checks app.Environment.IsProduction()
    When the plugin Configure method executes
    Then the plugin should skip development features                                   # UAC043
    And the plugin should only register production-ready endpoints                     # UAC044

  @SC16
  Scenario: Plugin adds HTTPS redirection
    Given a plugin needs to enforce HTTPS
    When the plugin Configure method executes with WebApplication host
    Then the plugin should be able to call app.UseHttpsRedirection()                   # UAC045
    And HTTPS redirection should be active for plugin routes                           # UAC046

  @SC17
  Scenario: Plugin registers minimal API endpoints with parameters
    Given a plugin registers an endpoint with route parameters
    When the plugin calls app.MapGet("/api/items/{id}", handler)
    Then the endpoint should accept the id parameter                                   # UAC047
    And the parameter should be bound correctly                                        # UAC048
    And the handler should receive the id value                                        # UAC049

  @SC18
  Scenario: Plugin registers POST endpoint with request body
    Given a plugin registers a POST endpoint
    When the plugin calls app.MapPost("/api/items", handler)
    Then the endpoint should accept POST requests                                      # UAC050
    And the request body should be deserialized correctly                              # UAC051
    And the handler should process the request data                                    # UAC052

  @SC19 @EdgeCase
  Scenario: Plugin configuration with null host falls back gracefully
    Given a plugin is registered in a non-ASP.NET Core context
    When Configure is called with host set to null
    Then the plugin should detect the null host                                        # UAC053
    And the plugin should skip WebApplication-specific configuration                   # UAC054
    And the plugin should return Task.CompletedTask                                    # UAC055

  @SC20 @Reflection
  Scenario: ASP.NET Core plugin extension methods are available
    Given I have an IServiceCollection instance
    Then AddPlugins() should be available as an extension method                       # UAC056
    And AddPlugin<T>() should be available as an extension method                      # UAC057
    And AddPlugin(IPlugin) should be available as an extension method                  # UAC058

  @SC21 @Reflection
  Scenario: ASP.NET Core application extension methods are available
    Given I have a WebApplication instance
    Then UsePlugins() should be available as an extension method                       # UAC059
    And it should accept an optional host parameter                                    # UAC060

  @SC22
  Scenario: Plugin lifetime in ASP.NET Core
    Given a plugin is registered in the service collection
    When the plugin is resolved from the service provider
    Then the plugin should have the correct lifetime (singleton/scoped/transient)      # UAC061
    And the same instance should be used throughout the application lifetime if singleton  # UAC062
    And the plugin Configure should only execute once                                  # UAC063

  @SC23 @EdgeCase
  Scenario: Multiple calls to UsePlugins should be idempotent
    Given plugins have been registered
    And app.UsePlugins() has been called once
    When app.UsePlugins() is called a second time
    Then the behavior should be deterministic and documented                           # UAC064
    And plugins should handle multiple calls gracefully                                # UAC065

  @SC24 @Configuration
  Scenario: Plugin integrates with ASP.NET Core configuration system
    Given a plugin needs configuration settings
    And the appsettings.json contains plugin-specific settings
    When the plugin Configure method executes
    Then the plugin should be able to resolve IConfiguration                           # UAC066
    And the plugin should be able to read its configuration section                    # UAC067
    And the plugin should apply the configuration settings                             # UAC068

  @SC25
  Scenario: Plugin uses dependency injection for its own services
    Given a BackendPlugin registers a WeatherService during Install
    And the service is registered as a singleton
    When the plugin Configure executes
    Then the plugin should be able to resolve WeatherService                           # UAC069
    And the service should be used in endpoint handlers                                # UAC070
    And the same instance should be used across requests                               # UAC071

  @SC26
  Scenario: Sample API integration test
    Given the sample API application is configured
    And BackendPlugin is registered via builder.Services.AddPlugin<BackendPlugin>()
    And builder.Services.AddPlugins() loads additional plugins from config
    And builder.Services.AddOpenApi() is called
    When the application is built and started
    Then the /weatherforecast endpoint should be accessible                            # UAC072
    And the OpenAPI documentation should be available in Development                   # UAC073
    And HTTPS redirection should be active                                             # UAC074

  @SC27
  Scenario: Plugin GlobalUsings are properly configured
    Given a plugin project references ASP.NET Core
    And the GlobalUsings.cs includes common namespaces
    When plugin code is written
    Then ASP.NET Core types should be available without explicit using statements      # UAC075
    And the plugin should compile successfully                                         # UAC076
