# --------------------------------------------------------------------
# Feature: Plugin Discovery and Loading
# --------------------------------------------------------------------

@VCHIP-0010 @UC03 @Plugin @Discovery @Loading
Feature: Plugin Discovery and Loading
  As a developer using the LowlandTech Plugin Framework
  I want the system to discover and load plugins from assemblies
  So that I can extend my application with modular functionality

  Background:
    Given the plugin framework is initialized
    And the application has access to a plugins directory

  @SC01
  Scenario: Load a single plugin from configuration
    Given the configuration contains a plugin entry for "LowlandTech.Sample.Backend"
    And the plugin "LowlandTech.Sample.Backend" is marked as active
    And the plugin assembly exists at the expected path
    When the plugin discovery process runs
    Then the plugin "LowlandTech.Sample.Backend" should be loaded successfully          # UAC001
    And the plugin should be registered in the service collection                       # UAC002
    And the plugin should have IsActive set to true                                     # UAC003

  @SC02
  Scenario: Skip loading inactive plugins
    Given the configuration contains a plugin entry for "LowlandTech.Sample.Backend"
    And the plugin "LowlandTech.Sample.Backend" is marked as inactive
    When the plugin discovery process runs
    Then the plugin "LowlandTech.Sample.Backend" should not be loaded                   # UAC004
    And the plugin should not be registered in the service collection                   # UAC005

  @SC03
  Scenario: Load multiple plugins from configuration
    Given the configuration contains the following plugin entries:
      | Name                          | IsActive |
      | LowlandTech.Sample.Backend    | true     |
      | LowlandTech.Sample.Frontend   | true     |
      | LowlandTech.Sample.Reporting  | true     |
    And all plugin assemblies exist at their expected paths
    When the plugin discovery process runs
    Then all 3 plugins should be loaded successfully                                    # UAC006
    And all 3 plugins should be registered in the service collection                    # UAC007

  @SC04
  Scenario: Handle mixed active and inactive plugins
    Given the configuration contains the following plugin entries:
      | Name                          | IsActive |
      | LowlandTech.Sample.Backend    | true     |
      | LowlandTech.Sample.Frontend   | false    |
      | LowlandTech.Sample.Reporting  | true     |
    And all plugin assemblies exist at their expected paths
    When the plugin discovery process runs
    Then 2 plugins should be loaded successfully                                        # UAC008
    And only active plugins should be registered in the service collection              # UAC009
    And the plugin "LowlandTech.Sample.Frontend" should not be loaded                   # UAC010

  @SC05 @ErrorHandling
  Scenario: Fail gracefully when plugin assembly is missing
    Given the configuration contains a plugin entry for "LowlandTech.Missing.Plugin"
    And the plugin "LowlandTech.Missing.Plugin" is marked as active
    But the plugin assembly does not exist at the expected path
    When the plugin discovery process runs
    Then an error should be logged for "LowlandTech.Missing.Plugin"                     # UAC011
    And the application should continue running                                         # UAC012
    And no instance of "LowlandTech.Missing.Plugin" should be registered                # UAC013

  @SC06 @ErrorHandling
  Scenario: Fail gracefully when plugin assembly has no IPlugin implementation
    Given the configuration contains a plugin entry for "LowlandTech.Invalid.Plugin"
    And the plugin "LowlandTech.Invalid.Plugin" is marked as active
    And the plugin assembly exists but contains no types implementing IPlugin
    When the plugin discovery process runs
    Then an error should be logged indicating no IPlugin implementation was found       # UAC014
    And the application should continue running                                         # UAC015
    And no plugin instance should be registered                                         # UAC016

  @SC07 @DirectRegistration
  Scenario: Load plugin by direct registration
    Given I have a plugin instance of type "BackendPlugin"
    When I call AddPlugin with the plugin instance
    Then the plugin should be registered in the service collection immediately          # UAC017
    And the plugin should be available for dependency injection                         # UAC018

  @SC08 @AspNetCore @DirectRegistration
  Scenario: Load plugin by type registration (ASP.NET Core)
    Given I have a plugin type "BackendPlugin"
    When I call AddPlugin<BackendPlugin>()
    Then the plugin type should be instantiated                                         # UAC019
    And the plugin should be registered in the service collection                       # UAC020
    And the plugin should be available for dependency injection                         # UAC021

  @SC09 @AssemblyLoading
  Scenario: Plugin assembly is loaded into correct AssemblyLoadContext
    Given the configuration contains a plugin entry for "LowlandTech.Sample.Backend"
    And the plugin "LowlandTech.Sample.Backend" is marked as active
    When the plugin discovery process runs
    Then the plugin assembly should be loaded into the current AssemblyLoadContext      # UAC022
    And the assembly should be accessible from the plugin's Assemblies collection       # UAC023

  @SC10 @Reflection
  Scenario: Discover plugin type using reflection
    Given a plugin assembly contains a type implementing IPlugin
    And the plugin type is decorated with a PluginId attribute
    When the type discovery process scans the assembly
    Then the plugin type should be identified correctly                                 # UAC024
    And an instance of the plugin type should be created using Activator                # UAC025

  @SC11 @EdgeCase
  Scenario: Handle empty plugins configuration section
    Given the configuration has an empty "Plugins" section
    When the plugin discovery process runs
    Then no plugins should be loaded                                                    # UAC026
    And no errors should be logged                                                      # UAC027
    And the application should continue running normally                                # UAC028

  @SC12 @EdgeCase
  Scenario: Handle missing plugins configuration section
    Given the configuration does not contain a "Plugins" section
    When the plugin discovery process runs
    Then no plugins should be loaded                                                    # UAC029
    And no errors should be logged                                                      # UAC030
    And the application should continue running normally                                # UAC031
