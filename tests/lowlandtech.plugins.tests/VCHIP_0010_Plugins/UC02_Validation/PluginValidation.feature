# --------------------------------------------------------------------
# Feature: Plugin Validation and Guard Clauses
# --------------------------------------------------------------------

@VCHIP-0010 @UC02 @Plugin @Validation
Feature: Plugin Validation and Guard Clauses
  As a developer using the LowlandTech Plugin Framework
  I want the framework to validate plugins before registration
  So that I can catch configuration errors early and ensure plugin integrity

  Background:
    Given the plugin framework is initialized
    And guard clauses are enabled

  @SC01
  Scenario: Validate plugin has PluginId attribute
    Given a plugin class is decorated with [PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
    When the plugin is validated using Guard.Against.MissingPluginId
    Then the validation should pass                                                    # UAC001
    And the plugin ID should be extracted successfully                                 # UAC002
    And the ID should be "306b92e3-2db6-45fb-99ee-9c63b090f3fc"                        # UAC003

  @SC02 @ErrorHandling
  Scenario: Reject plugin without PluginId attribute
    Given a plugin class is not decorated with a PluginId attribute
    When the plugin is validated using Guard.Against.MissingPluginId
    Then an ArgumentException should be thrown                                         # UAC004
    And the exception message should indicate "Invalid plugin id, it must be provided"  # UAC005
    And the plugin should not be registered                                            # UAC006

  @SC03 @ErrorHandling
  Scenario: Validate plugin is not null
    Given a null plugin reference
    When Guard.Against.MissingPluginId is called with the null plugin
    Then an ArgumentNullException should be thrown for the plugin parameter            # UAC007
    And the validation should fail before checking the PluginId attribute              # UAC008

  @SC04
  Scenario: Custom error message for missing PluginId
    Given a plugin without a PluginId attribute
    When Guard.Against.MissingPluginId is called with a custom message "Plugin requires unique identifier"
    Then an ArgumentException should be thrown                                         # UAC009
    And the exception message should be "Plugin requires unique identifier"            # UAC010

  @SC05
  Scenario: Validate PluginId format is a valid GUID
    Given a plugin with PluginId attribute containing "306b92e3-2db6-45fb-99ee-9c63b090f3fc"
    When the plugin ID is extracted
    Then the ID should be a valid GUID format                                          # UAC011
    And the ID should be usable for plugin identification                              # UAC012

  @SC06
  Scenario: Validate PluginId attribute is unique across plugins
    Given multiple plugins are registered:
      | Plugin Class      | Plugin ID                                |
      | BackendPlugin     | 306b92e3-2db6-45fb-99ee-9c63b090f3fc     |
      | FrontendPlugin    | 4a8c6f2e-1b3d-4e5f-9a7c-8d6e5f4a3b2c     |
    When the plugins are validated
    Then each plugin should have a unique ID                                           # UAC013
    And no ID conflicts should exist                                                   # UAC014

  @SC07 @ErrorHandling
  Scenario: Reject plugins with duplicate PluginId
    Given two plugins have the same PluginId "306b92e3-2db6-45fb-99ee-9c63b090f3fc"
    When both plugins are registered
    Then a validation error should occur                                               # UAC015
    And the second plugin should fail to register                                      # UAC016
    And the duplicate ID should be reported                                            # UAC017

  @SC08
  Scenario: Guard clause validates parameter name correctly
    Given a plugin without PluginId
    When Guard.Against.MissingPluginId is called with parameter name "testPlugin"
    Then the ArgumentException should reference parameter "testPlugin"                 # UAC018
    And the error should clearly indicate which parameter failed validation            # UAC019

  @SC09
  Scenario: Validate plugin instance during AddPlugin
    Given I attempt to add a plugin using AddPlugin method
    And the plugin is missing the PluginId attribute
    When the AddPlugin method executes guard validation
    Then the plugin should be rejected before registration                             # UAC020
    And an ArgumentException should be thrown                                          # UAC021
    And the service collection should not contain the invalid plugin                   # UAC022

  @SC10 @EdgeCase
  Scenario: Validate plugin metadata with empty Name
    Given a plugin instance with the following metadata:
      | Property    | Value                                    |
      | Id          | 306b92e3-2db6-45fb-99ee-9c63b090f3fc     |
      | Name        |                                          |
      | IsActive    | true                                     |
    When the plugin is validated
    Then the Id should be valid                                                        # UAC023
    And the plugin should be accepted if Name is optional                              # UAC024
    And a warning might be issued for empty Name                                       # UAC025

  @SC11
  Scenario: Validate plugin with empty Name gets default value
    Given a plugin with Name set to empty string
    When the plugin is registered
    Then a default name should be assigned                                             # UAC026
    And the validation behavior should be consistent                                   # UAC027

  @SC12 @EdgeCase
  Scenario: Guard clause handles null parameter name
    Given a plugin without PluginId
    When Guard.Against.MissingPluginId is called with null parameter name
    Then an appropriate exception should be thrown                                     # UAC028
    And the guard should handle the null parameter name gracefully                     # UAC029

  @SC13
  Scenario: Validate multiple plugins in batch
    Given a collection of plugins to validate:
      | Plugin Class      | Has PluginId |
      | BackendPlugin     | true         |
      | FrontendPlugin    | true         |
      | InvalidPlugin     | false        |
    When each plugin is validated with Guard.Against.MissingPluginId
    Then BackendPlugin should pass validation                                          # UAC030
    And FrontendPlugin should pass validation                                          # UAC031
    And InvalidPlugin should fail validation                                           # UAC032
    And only valid plugins should be registered                                        # UAC033

  @SC14 @Reflection
  Scenario: PluginId attribute can be retrieved via reflection
    Given a plugin class BackendPlugin with PluginId attribute
    When I use Attribute.GetCustomAttribute to retrieve the PluginId
    Then the PluginId attribute should be found                                        # UAC034
    And the attribute type should be PluginId                                          # UAC035
    And the Id property should be accessible                                           # UAC036

  @SC15
  Scenario: Validate plugin inheritance from Plugin base class
    Given a custom plugin class inherits from Plugin base class
    When the plugin is instantiated
    Then the plugin should implement IPlugin interface                                 # UAC037
    And all required properties should be available                                    # UAC038
    And all abstract methods should be implemented                                     # UAC039

  @SC16 @Reflection
  Scenario: Validate plugin implements IPlugin interface
    Given a plugin type is discovered from an assembly
    When the type is checked for IPlugin implementation
    Then typeof(IPlugin).IsAssignableFrom(pluginType) should return true               # UAC040
    And the plugin should be eligible for registration                                 # UAC041

  @SC17
  Scenario: Guard extension method is available on IGuardClause
    Given I have an IGuardClause instance
    When I call the MissingPluginId extension method
    Then the method should be available as an extension                                # UAC042
    And it should integrate with Ardalis.GuardClauses library                          # UAC043

  @SC18
  Scenario: Validate plugin with all required attributes and implementations
    Given a fully compliant plugin:
      And Decorated with [PluginId("306b92e3-2db6-45fb-99ee-9c63b090f3fc")]
      And Inherits from Plugin base class
      And Implements Install method
      And Implements Configure method
    When the plugin goes through all validations
    Then all validations should pass                                                   # UAC044
    And the plugin should be successfully registered                                   # UAC045
    And the plugin should be ready for use                                             # UAC046

  @SC19 @ErrorHandling
  Scenario: Validation during plugin type discovery
    Given plugin discovery is scanning an assembly
    And the assembly contains a type with no PluginId
    When the validation runs during discovery
    Then the invalid type should be skipped                                            # UAC047
    And an error should be logged                                                      # UAC048
    And other valid plugins in the assembly should still be loaded                     # UAC049

  @SC20
  Scenario: Guard clause provides helpful error messages
    Given a plugin fails MissingPluginId validation
    When the ArgumentException is thrown
    Then the error message should clearly state the problem                            # UAC050
    And the error should include the parameter name                                    # UAC051
    And the error should help developers fix the issue quickly                         # UAC052
