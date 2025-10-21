# --------------------------------------------------------------------
# Feature: Plugin Configuration
# --------------------------------------------------------------------

@VCHIP-0010 @UC00 @Plugin @Configuration
Feature: Plugin Configuration
  As a developer using the LowlandTech Plugin Framework
  I want to configure plugins through appsettings.json
  So that I can control plugin behavior without code changes

  Background:
    Given the plugin framework is initialized
    And the application uses configuration files

  @SC01
  Scenario: Load plugin configuration from appsettings.json
    Given the appsettings.json contains a "Plugins" section
    And the "Plugins" section contains plugin entries
    When the configuration is read
    Then the PluginOptions should be populated                                         # UAC001
    And the PluginOptions.Plugins list should contain all configured plugins           # UAC002

  @SC02
  Scenario: Parse plugin name from configuration
    Given the appsettings.json contains the following plugin configuration:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": true
          }
        ]
      }
      """
    When the configuration is parsed into PluginConfig objects
    Then the plugin name should be "LowlandTech.Sample.Backend"                        # UAC003
    And the name should not include the .dll extension                                 # UAC004

  @SC03
  Scenario: Parse IsActive flag from configuration
    Given the appsettings.json contains the following plugin configuration:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": false
          }
        ]
      }
      """
    When the configuration is parsed into PluginConfig objects
    Then the plugin IsActive flag should be false                                      # UAC005
    And the plugin should not be loaded                                                # UAC006

  @SC04
  Scenario: Use PluginOptions constant for configuration section name
    Given I need to bind plugin configuration from appsettings
    When I use the PluginOptions.Name constant
    Then the constant should equal "Plugins"                                           # UAC007
    And the configuration should bind correctly using this constant                    # UAC008

  @SC05
  Scenario: Handle multiple plugin configurations
    Given the appsettings.json contains the following configuration:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": true
          },
          {
            "Name": "LowlandTech.Sample.Frontend",
            "IsActive": true
          },
          {
            "Name": "LowlandTech.Sample.Reporting",
            "IsActive": false
          }
        ]
      }
      """
    When the configuration is parsed
    Then 3 PluginConfig objects should be created                                      # UAC009
    And 2 plugins should be marked as active                                           # UAC010
    And 1 plugin should be marked as inactive                                          # UAC011

  @SC06 @EdgeCase
  Scenario: Plugin configuration with empty plugins array
    Given the appsettings.json contains the following configuration:
      """
      {
        "Plugins": []
      }
      """
    When the configuration is parsed
    Then the PluginOptions.Plugins list should be empty                                # UAC012
    And no plugins should be loaded                                                    # UAC013
    And no errors should occur                                                         # UAC014

  @SC07
  Scenario: Environment-specific plugin configuration (Development)
    Given the application is running in Development environment
    And the appsettings.Development.json overrides plugin configuration:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": true
          },
          {
            "Name": "LowlandTech.Debug.Plugin",
            "IsActive": true
          }
        ]
      }
      """
    When the configuration is loaded with environment override
    Then the Development-specific plugins should be active                             # UAC015
    And the debug plugin should be loaded only in Development                          # UAC016

  @SC08
  Scenario: Environment-specific plugin configuration (Production)
    Given the application is running in Production environment
    And the appsettings.Production.json contains:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": true
          },
          {
            "Name": "LowlandTech.Debug.Plugin",
            "IsActive": false
          }
        ]
      }
      """
    When the configuration is loaded with environment override
    Then the debug plugin should be inactive in Production                             # UAC017
    And only production-ready plugins should be loaded                                 # UAC018

  @SC09 @Validation @ErrorHandling
  Scenario: Plugin configuration validation - missing Name
    Given the appsettings.json contains a plugin entry with no Name:
      """
      {
        "Plugins": [
          {
            "IsActive": true
          }
        ]
      }
      """
    When the configuration is parsed and validated
    Then the plugin should be skipped during loading                                   # UAC019
    And a configuration validation error should be logged                              # UAC020

  @SC10
  Scenario: Plugin configuration with default IsActive value
    Given a PluginConfig is created without specifying IsActive
    When the PluginConfig is initialized
    Then the IsActive property should have a default value                             # UAC021
    And the default behavior should be documented                                      # UAC022

  @SC11
  Scenario: Reload plugin configuration at runtime
    Given the application is running with loaded plugins
    And the appsettings.json is modified to add a new plugin
    When the configuration is reloaded
    Then the new plugin configuration should be detected                               # UAC023
    And the application should be able to reload plugins (if supported)                # UAC024

  @SC12 @EdgeCase
  Scenario: Plugin configuration with extra properties
    Given the appsettings.json contains additional properties:
      """
      {
        "Plugins": [
          {
            "Name": "LowlandTech.Sample.Backend",
            "IsActive": true,
            "CustomProperty": "value"
          }
        ]
      }
      """
    When the configuration is parsed
    Then the standard properties should be read correctly                              # UAC025
    And extra properties should be ignored without errors                              # UAC026
    And the plugin should load successfully                                            # UAC027

  @SC13
  Scenario: Plugin path resolution from configuration
    Given the plugin name is "LowlandTech.Sample.Backend"
    And the current assembly directory is "C:\App\bin"
    When the plugin path is constructed
    Then the path should be "C:\App\bin\LowlandTech.Sample.Backend.dll"               # UAC028
    And the .dll extension should be appended automatically                            # UAC029

  @SC14
  Scenario: Plugin configuration with complex scenarios
    Given the appsettings.json contains:
      """
      {
        "Plugins": [
          {
            "Name": "Plugin.One",
            "IsActive": true
          },
          {
            "Name": "Plugin.Two",
            "IsActive": false
          },
          {
            "Name": "Plugin.Three",
            "IsActive": true
          }
        ]
      }
      """
    When plugins are loaded from configuration
    Then only "Plugin.One" and "Plugin.Three" should be loaded                         # UAC030
    And "Plugin.Two" should be ignored                                                 # UAC031
    And the loaded plugins should be in the order they were configured                 # UAC032

  @SC15
  Scenario: PluginOptions initialization with default values
    Given a new PluginOptions instance is created
    When no configuration is provided
    Then the Plugins list should be initialized as an empty list                       # UAC033
    And the list should not be null                                                    # UAC034

  @SC16
  Scenario: Configuration binding with IConfiguration
    Given I have an IConfiguration instance with plugin settings
    When I bind the configuration to PluginOptions using the "Plugins" section
    Then the PluginOptions should be populated correctly                               # UAC035
    And all plugin configurations should be deserialized properly                      # UAC036

  @SC17
  Scenario: Case sensitivity in configuration section names
    Given the configuration section is named "Plugins" with capital P
    When I use PluginOptions.Name to retrieve the section
    Then the configuration should bind correctly                                       # UAC037
    And the section name should match exactly                                          # UAC038
