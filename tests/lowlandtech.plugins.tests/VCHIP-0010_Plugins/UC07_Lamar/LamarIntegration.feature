# --------------------------------------------------------------------
# Feature: Lamar Container Integration
# --------------------------------------------------------------------

@VCHIP-0010 @UC07 @Plugin @Lamar
Feature: Lamar Container Integration
  As a developer using Lamar IoC container
  I want to integrate plugins with Lamar
  So that I can use plugins in non-ASP.NET Core applications

  Background:
    Given a Lamar ServiceRegistry is initialized
    And the core LowlandTech.Plugins package is referenced

  @SC01
  Scenario: Add plugins from configuration to ServiceRegistry
    Given the configuration contains active plugins
    When I call services.AddPlugins() on the ServiceRegistry
    Then plugins should be discovered from configuration                               # UAC001
    And plugins should be registered in the ServiceRegistry                            # UAC002
    And the Install method should be called on each plugin                             # UAC003

  @SC02 @DirectRegistration
  Scenario: Add a specific plugin instance to ServiceRegistry
    Given I have a plugin instance
    When I call services.AddPlugin(pluginInstance) on the ServiceRegistry
    Then the plugin should be registered in the ServiceRegistry                        # UAC004
    And the plugin Install method should be called                                     # UAC005
    And the plugin should be available in the container                                # UAC006

  @SC03
  Scenario: Use plugin by type with Lamar
    Given I have a plugin type
    And I have a Lamar Container built from ServiceRegistry
    When I call services.UsePlugin<TPlugin>(host) on the ServiceRegistry
    Then the plugin type should be registered                                          # UAC007
    And the plugin should be instantiated                                              # UAC008
    And the plugin Configure method should be called with the container                # UAC009

  @SC04
  Scenario: UsePlugins on Lamar IContainer
    Given plugins have been registered in ServiceRegistry
    And the Lamar Container has been built
    When I call container.UsePlugins()
    Then all registered plugins should be retrieved from the container                 # UAC010
    And the Configure method should be called on each plugin                           # UAC011
    And each plugin should receive the IContainer as service provider                  # UAC012

  @SC05
  Scenario: UsePlugins with custom host object
    Given plugins are registered in a Lamar container
    And I have a custom host object
    When I call container.UsePlugins(customHost)
    Then each plugin Configure method should receive the custom host                   # UAC013
    And plugins should be able to configure themselves with the host                   # UAC014

  @SC06
  Scenario: Plugin registers services in Lamar ServiceRegistry
    Given a plugin needs to register services
    When the plugin Install method is called with ServiceRegistry
    Then the plugin should be able to use Lamar-specific registration syntax           # UAC015
    And the plugin should register services using For<T>().Use<TImpl>()                # UAC016
    And services should be available after container is built                          # UAC017

  @SC07
  Scenario: Resolve all plugins from Lamar container
    Given multiple plugins are registered in ServiceRegistry
    And the container is built
    When I call container.GetAllInstances<IPlugin>()
    Then all registered plugin instances should be returned                            # UAC018
    And the collection should contain all active plugins                               # UAC019

  @SC08
  Scenario: Plugin with Lamar-specific features
    Given a plugin uses Lamar's advanced registration features
    When the plugin Install method uses ServiceRegistry features like:
      - Policies
      - Interceptors
      - Decorated instances
    Then the Lamar-specific features should work correctly                             # UAC020
    And the plugin should integrate seamlessly                                         # UAC021

  @SC09 @Reflection
  Scenario: Lamar ServiceRegistry extension methods are available
    Given I have a ServiceRegistry instance
    Then AddPlugins() should be available as an extension method                       # UAC022
    And AddPlugin(IPlugin) should be available as an extension method                  # UAC023
    And UsePlugin<T>(host) should be available as an extension method                  # UAC024

  @SC10 @Reflection
  Scenario: Lamar IContainer extension methods are available
    Given I have an IContainer instance
    Then UsePlugins() should be available as an extension method                       # UAC025
    And UsePlugins(host) should be available with optional host parameter              # UAC026

  @SC11 @Lifecycle
  Scenario: Plugin lifecycle with Lamar Container
    Given a plugin is registered in ServiceRegistry
    When the Container is built
    And UsePlugins is called on the container
    Then the plugin Install should have been called during registration                # UAC027
    And the plugin Configure should be called when UsePlugins executes                 # UAC028
    And the plugin should have full access to Lamar container features                 # UAC029

  @SC12
  Scenario: Multiple plugins with Lamar dependency chain
    Given Plugin A registers ServiceA
    And Plugin B depends on ServiceA
    And both plugins are registered in ServiceRegistry
    When the container is built and UsePlugins is called
    Then Plugin A services should be available to Plugin B                             # UAC030
    And the dependency chain should resolve correctly                                  # UAC031
    And no circular dependencies should occur                                          # UAC032

  @SC13
  Scenario: Lamar scoped services in plugins
    Given a plugin registers a scoped service
    When the service is resolved from the container
    Then the scoped service should follow Lamar scoping rules                          # UAC033
    And different scopes should get different instances                                # UAC034
    And services should be disposed when scope ends                                    # UAC035

  @SC14
  Scenario: Plugin with Lamar singleton registration
    Given a plugin registers a singleton service
    When the service is resolved multiple times
    Then the same instance should be returned each time                                # UAC036
    And the singleton should live for the container lifetime                           # UAC037

  @SC15
  Scenario: UsePlugin method installs and configures plugin
    Given I have a plugin type TPlugin
    And I have a ServiceRegistry
    When I call services.UsePlugin<TPlugin>(host)
    Then the plugin should be registered in the ServiceRegistry                        # UAC038
    And configuration should be handled based on container availability                # UAC039

  @SC16
  Scenario: Lamar integration without ASP.NET Core
    Given I'm building a console application with Lamar
    And I want to use the plugin system
    When I use the core LowlandTech.Plugins package
    Then I should be able to register and use plugins                                  # UAC040
    And ASP.NET Core dependencies should not be required                               # UAC041
    And the plugin system should work standalone                                       # UAC042

  @SC17
  Scenario: Plugin configuration with Lamar GetInstance
    Given a plugin needs to resolve services during Configure
    When the plugin calls container.GetInstance<TService>()
    Then the service should be resolved from the Lamar container                       # UAC043
    And the plugin should receive the correct instance                                 # UAC044

  @SC18 @Validation
  Scenario: Lamar container validation with plugins
    Given plugins register complex dependency graphs
    When the Lamar container is built with validation enabled
    Then the container should validate all registrations                               # UAC045
    And any misconfigured services should be detected                                  # UAC046
    And appropriate errors should be thrown for validation failures                    # UAC047

  @SC19 @Reflection
  Scenario: Plugin assemblies are tracked in Lamar
    Given a plugin is loaded from an external assembly
    When the plugin is registered in the ServiceRegistry
    Then the plugin Assemblies collection should contain the loaded assembly           # UAC048
    And the assembly should be accessible for type scanning or other operations        # UAC049
