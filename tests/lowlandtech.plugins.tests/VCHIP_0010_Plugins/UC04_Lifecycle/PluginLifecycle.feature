# --------------------------------------------------------------------
# Feature: Plugin Lifecycle Management
# --------------------------------------------------------------------

@VCHIP-0010 @UC04 @Plugin @Lifecycle
Feature: Plugin Lifecycle Management
  As a developer using the LowlandTech Plugin Framework
  I want plugins to follow a well-defined lifecycle
  So that I can properly initialize and configure plugin functionality

  Background:
    Given the plugin framework is initialized
    And a plugin implementing IPlugin is available

  @SC01
  Scenario: Execute Install phase during plugin registration
    Given a plugin is being registered in the service collection
    When the Install method is called
    Then the plugin should register its services in the service collection             # UAC001
    And the plugin services should be available for dependency injection               # UAC002
    And the Install method should complete successfully                                # UAC003

  @SC02
  Scenario: Execute ConfigureContext phase asynchronously
    Given a plugin has been installed
    And the plugin implements ConfigureContext method
    When the ConfigureContext method is called with the service collection
    Then the plugin should perform asynchronous context configuration                  # UAC004
    And the ConfigureContext method should complete without errors                     # UAC005
    And the method should return a completed Task                                      # UAC006

  @SC03
  Scenario: Execute Configure phase with service provider
    Given a plugin has been installed
    And the service provider has been built
    When the Configure method is called with the service provider
    Then the plugin should have access to all registered services                      # UAC007
    And the plugin should complete its configuration                                   # UAC008
    And the Configure method should return a completed Task                            # UAC009

  @SC04 @AspNetCore
  Scenario: Execute Configure phase with host object (ASP.NET Core)
    Given a plugin has been installed in an ASP.NET Core application
    And the WebApplication has been built
    When the Configure method is called with the service provider and WebApplication host
    Then the plugin should have access to the WebApplication instance                  # UAC010
    And the plugin should be able to configure routes                                  # UAC011
    And the plugin should be able to configure middleware                              # UAC012
    And the Configure method should complete successfully                              # UAC013

  @SC05
  Scenario: Complete lifecycle flow - Install, ConfigureContext, Configure
    Given a new plugin is being added to the application
    When the complete lifecycle executes in order:
      | Phase            |
      | Install          |
      | ConfigureContext |
      | Configure        |
    Then the Install phase should execute first                                        # UAC014
    And the ConfigureContext phase should execute after Install                        # UAC015
    And the Configure phase should execute last                                        # UAC016
    And all phases should complete successfully                                        # UAC017

  @SC06
  Scenario: Plugin registers services during Install
    Given a plugin needs to register custom services
    When the Install method is called with the service collection
    Then the plugin should add singleton services to the collection                    # UAC018
    And the plugin should add scoped services to the collection                        # UAC019
    And the plugin should add transient services to the collection                     # UAC020
    And all registered services should be resolvable after the container is built      # UAC021

  @SC07
  Scenario: Plugin with virtual ConfigureContext uses default implementation
    Given a plugin does not override the ConfigureContext method
    When the ConfigureContext method is called
    Then the default virtual implementation should execute                             # UAC022
    And the method should return an immediately completed Task                         # UAC023
    And no errors should occur                                                         # UAC024

  @SC08
  Scenario: UsePlugins executes Configure on all loaded plugins
    Given multiple plugins are registered in the service collection:
      | Plugin Name               |
      | BackendPlugin             |
      | FrontendPlugin            |
      | ReportingPlugin           |
    And all plugins have been installed
    When UsePlugins is called on the container/application
    Then the Configure method should be called on each plugin                          # UAC025
    And each plugin should receive the service provider                                # UAC026
    And each plugin should receive the optional host object                            # UAC027
    And all plugins should complete configuration successfully                         # UAC028

  @SC09 @EdgeCase
  Scenario: Plugin Configure phase with null host
    Given a plugin is being used in a non-ASP.NET Core application
    When the Configure method is called with host set to null
    Then the plugin should handle the null host gracefully                             # UAC029
    And the plugin should skip host-specific configuration                             # UAC030
    And the Configure method should return successfully                                # UAC031

  @SC10
  Scenario: Plugin accesses dependencies from service provider during Configure
    Given a plugin has registered services during Install
    And the service provider has been built with those services
    When the Configure method is called with the service provider
    Then the plugin should be able to resolve its registered services                  # UAC032
    And the plugin should be able to use those services for configuration              # UAC033
    And all service resolutions should succeed                                         # UAC034

  @SC11 @ErrorHandling
  Scenario: Handle exception during Install phase
    Given a plugin throws an exception during Install
    When the plugin registration process attempts to call Install
    Then the exception should be propagated                                            # UAC035
    And the plugin registration should fail                                            # UAC036
    And subsequent plugins should not be affected                                      # UAC037

  @SC12 @ErrorHandling
  Scenario: Handle exception during ConfigureContext phase
    Given a plugin throws an exception during ConfigureContext
    When the ConfigureContext method is called
    Then the exception should be propagated                                            # UAC038
    And the plugin should not proceed to Configure phase                               # UAC039
    And the error should be handled by the application                                 # UAC040

  @SC13 @ErrorHandling
  Scenario: Handle exception during Configure phase
    Given a plugin throws an exception during Configure
    When the UsePlugins method calls Configure on the plugin
    Then the exception should be propagated                                            # UAC041
    And subsequent plugins should still be processed (or fail fast depending on implementation)  # UAC042
    And the error should be logged or handled appropriately                            # UAC043

  @SC14
  Scenario: Plugin metadata is preserved throughout lifecycle
    Given a plugin has metadata properties set:
      | Property    | Value                                    |
      | Id          | 306b92e3-2db6-45fb-99ee-9c63b090f3fc     |
      | Name        | BackendPlugin                            |
      | IsActive    | true                                     |
      | Description | Sample backend plugin                    |
      | Company     | LowlandTech                              |
      | Version     | 1.0.0                                    |
    When the plugin goes through all lifecycle phases
    Then all metadata properties should remain unchanged                               # UAC044
    And the metadata should be accessible at any lifecycle phase                       # UAC045

  @SC15
  Scenario: Multiple plugins execute lifecycle independently
    Given plugin A and plugin B are both registered
    When plugin A is executing its Install phase
    Then plugin B should not be affected                                               # UAC046
    And plugin B should execute its own lifecycle phases independently                 # UAC047
    And both plugins should complete their lifecycles successfully                     # UAC048

  @SC16 @Reflection
  Scenario: Plugin Assemblies collection is available during lifecycle
    Given a plugin has loaded assemblies in its Assemblies collection
    When the plugin executes through its lifecycle phases
    Then the Assemblies collection should be accessible in Install                     # UAC049
    And the Assemblies collection should be accessible in ConfigureContext             # UAC050
    And the Assemblies collection should be accessible in Configure                    # UAC051
    And the collection should not be serialized to JSON                                # UAC052
