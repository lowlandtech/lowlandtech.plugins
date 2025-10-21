# --------------------------------------------------------------------
# Feature: Error Handling and Edge Cases
# --------------------------------------------------------------------

@VCHIP-0010 @UC06 @Plugin @ErrorHandling
Feature: Error Handling and Edge Cases
  As a developer using the LowlandTech Plugin Framework
  I want the framework to handle errors gracefully
  So that my application remains stable even when plugins fail

  Background:
    Given the plugin framework is initialized
    And error handling mechanisms are in place

  @SC01 @EdgeCase
  Scenario: Handle plugin assembly load failure
    Given a plugin assembly path is invalid
    When the framework attempts to load the assembly
    Then an assembly load exception should be caught                                   # UAC001
    And an error should be logged with assembly path details                           # UAC002
    And the application should continue running                                        # UAC003
    And other plugins should still be loaded                                           # UAC004

  @SC02 @EdgeCase
  Scenario: Handle corrupted plugin assembly
    Given a plugin assembly file is corrupted
    When the framework attempts to load the assembly
    Then a BadImageFormatException should be caught                                    # UAC005
    And an error should be logged indicating corrupted assembly                        # UAC006
    And the plugin should be skipped                                                   # UAC007
    And the application should remain stable                                           # UAC008

  @SC03 @EdgeCase
  Scenario: Handle plugin with missing dependencies
    Given a plugin assembly has unresolved dependencies
    When the framework attempts to instantiate the plugin
    Then a FileNotFoundException or ReflectionTypeLoadException should occur           # UAC009
    And the error should clearly indicate the missing dependency                       # UAC010
    And the plugin should fail to load                                                 # UAC011
    And other plugins should not be affected                                           # UAC012

  @SC04 @EdgeCase
  Scenario: Handle plugin constructor exception
    Given a plugin constructor throws an exception
    When Activator.CreateInstance attempts to create the plugin
    Then the inner exception should be captured                                        # UAC013
    And the error should be logged with plugin type information                        # UAC014
    And the plugin should not be registered                                            # UAC015
    And the application should continue                                                # UAC016

  @SC05 @EdgeCase
  Scenario: Handle plugin Install method exception
    Given a plugin Install method throws an exception
    When the Install method is called during registration
    Then the exception should be propagated or caught based on policy                  # UAC017
    And the error should be logged with plugin name and stack trace                    # UAC018
    And the service collection may be in a partial state                               # UAC019
    And appropriate cleanup or rollback should occur if supported                      # UAC020

  @SC06 @EdgeCase
  Scenario: Handle plugin ConfigureContext exception
    Given a plugin ConfigureContext method throws an exception
    When the async ConfigureContext is awaited
    Then the exception should be captured                                              # UAC021
    And the error should be logged                                                     # UAC022
    And the plugin should not proceed to Configure phase                               # UAC023

  @SC07 @EdgeCase
  Scenario: Handle plugin Configure method exception
    Given a plugin Configure method throws an exception
    When UsePlugins iterates through plugins
    Then the exception should be caught or propagated                                  # UAC024
    And the error should be logged with plugin details                                 # UAC025
    And the error handling strategy should be determined by configuration              # UAC026

  @SC08 @EdgeCase
  Scenario: Handle circular dependency in plugin services
    Given Plugin A depends on Service B
    And Service B depends on Service A
    When the service provider attempts to resolve the circular dependency
    Then a circular dependency exception should be thrown                              # UAC027
    And the error message should indicate the dependency chain                         # UAC028
    And the application should fail to start or handle gracefully                      # UAC029

  @SC09 @EdgeCase
  Scenario: Handle plugin with abstract Install method not implemented
    Given a plugin class inherits from Plugin base class
    But does not implement the abstract Install method
    When the compiler processes the plugin
    Then a compilation error should occur                                              # UAC030
    And the plugin should not build                                                    # UAC031
    And the error should clearly state Install must be implemented                     # UAC032

  @SC10 @EdgeCase
  Scenario: Handle plugin with abstract Configure method not implemented
    Given a plugin class inherits from Plugin base class
    But does not implement the abstract Configure method
    When the compiler processes the plugin
    Then a compilation error should occur                                              # UAC033
    And the plugin should not build                                                    # UAC034
    And the error should clearly state Configure must be implemented                   # UAC035

  @SC11 @EdgeCase
  Scenario: Handle null service collection
    Given a null IServiceCollection is passed to AddPlugin
    When the method attempts to register the plugin
    Then a NullReferenceException or ArgumentNullException should be thrown            # UAC036
    And the error should clearly indicate the null parameter                           # UAC037

  @SC12 @EdgeCase
  Scenario: Handle null plugin instance
    Given a null plugin instance is passed to AddPlugin
    When the guard clause checks the plugin
    Then an ArgumentNullException should be thrown                                     # UAC038
    And the parameter name should be included in the exception                         # UAC039
    And the plugin should not be registered                                            # UAC040

  @SC13 @EdgeCase
  Scenario: Handle plugin with empty or whitespace Name
    Given a plugin is registered with an empty Name property
    When the plugin is loaded from configuration
    Then a validation warning should be logged                                         # UAC041
    And the plugin might use a default name                                            # UAC042
    And the plugin should still function if Name is not critical                       # UAC043

  @SC14 @EdgeCase
  Scenario: Handle plugin with duplicate Name
    Given two plugins have the same Name property
    When both plugins are registered
    Then both should be registered if Id is unique                                     # UAC044
    And the duplicate name should be logged as a warning                               # UAC045
    And the unique identifier should be used to differentiate them                     # UAC046

  @SC15 @EdgeCase
  Scenario: Handle plugin with IsActive set to false after registration
    Given a plugin is registered with IsActive = true
    When the IsActive property is changed to false at runtime
    Then the plugin behavior depends on implementation                                 # UAC047
    And changing IsActive at runtime may not affect already-configured plugins         # UAC048
    And proper handling should be documented                                           # UAC049

  @SC16 @EdgeCase
  Scenario: Handle missing configuration file
    Given the appsettings.json file does not exist
    When the application attempts to load plugin configuration
    Then the configuration should return empty or default values                       # UAC050
    And no plugins should be loaded from configuration                                 # UAC051
    And the application should continue without errors                                 # UAC052

  @SC17 @EdgeCase
  Scenario: Handle malformed JSON in configuration
    Given the appsettings.json contains invalid JSON syntax
    When the configuration is parsed
    Then a JsonException should be thrown                                              # UAC053
    And the error should indicate the JSON parsing failure                             # UAC054
    And the application should fail to start with a clear error message                # UAC055

  @SC18 @EdgeCase
  Scenario: Handle invalid PluginId GUID format
    Given a PluginId attribute contains an invalid GUID string
    When the plugin is loaded
    Then the Id should be treated as a string                                          # UAC056
    And the framework should validate the format appropriately                         # UAC057
    And the error should indicate the invalid format if validation fails               # UAC058

  @SC19 @EdgeCase
  Scenario: Handle plugin that takes too long to configure
    Given a plugin Configure method has an infinite loop or very long operation
    When UsePlugins calls Configure on the plugin
    Then the application may hang waiting for the plugin                               # UAC059
    And timeout mechanisms should be considered for production                         # UAC060
    And monitoring should detect hanging plugins                                       # UAC061

  @SC20 @EdgeCase
  Scenario: Handle plugin attempting to register duplicate route
    Given Plugin A registers a /api/data route
    And Plugin B also attempts to register /api/data route
    When both plugins configure their routes
    Then a route conflict exception should be thrown                                   # UAC062
    And the error should clearly indicate the duplicate route                          # UAC063
    And the application should fail to start or handle the conflict                    # UAC064

  @SC21 @EdgeCase
  Scenario: Handle plugin with version mismatch
    Given a plugin is built against framework version 1.0
    And the application uses framework version 2.0 with breaking changes
    When the plugin is loaded
    Then compatibility errors may occur at load time or runtime                        # UAC065
    And version compatibility should be checked and documented                         # UAC066
    And appropriate error messages should guide users                                  # UAC067

  @SC22 @EdgeCase @Reflection
  Scenario: Handle reflection failure during type discovery
    Given a plugin assembly contains types that cannot be loaded via reflection
    When GetTypes() is called on the assembly
    Then a ReflectionTypeLoadException may be thrown                                   # UAC068
    And the LoaderExceptions should be examined for details                            # UAC069
    And types that can be loaded should still be processed                             # UAC070

  @SC23 @EdgeCase
  Scenario: Handle case-sensitive file system issues
    Given a plugin name is "MyPlugin" in configuration
    But the actual file is "myplugin.dll" on a case-sensitive file system
    When the framework attempts to load the plugin
    Then a FileNotFoundException should occur                                          # UAC071
    And the error should indicate the case mismatch                                    # UAC072
    And the configuration should match exact file name casing                          # UAC073

  @SC24 @EdgeCase
  Scenario: Handle plugin assembly locked by another process
    Given a plugin assembly file is locked by another process
    When the framework attempts to load the assembly
    Then an IOException should occur                                                   # UAC074
    And the error should indicate the file is in use                                   # UAC075
    And the application should fail to load that plugin                                # UAC076

  @SC25 @EdgeCase
  Scenario: Handle plugin with insufficient permissions
    Given the application runs with limited file system permissions
    And a plugin assembly is in a restricted directory
    When the framework attempts to load the assembly
    Then an UnauthorizedAccessException should occur                                   # UAC077
    And the error should indicate permission denied                                    # UAC078
    And appropriate error logging should occur                                         # UAC079

  @SC26 @EdgeCase
  Scenario: Handle plugin service registration conflict
    Given Plugin A registers IService with ImplementationA
    And Plugin B registers IService with ImplementationB
    When both plugins install their services
    Then the behavior depends on DI container rules (last registration wins or multi-registration)  # UAC080
    And the conflict should be logged or documented                                    # UAC081
    And developers should be aware of service registration order                       # UAC082

  @SC27 @EdgeCase
  Scenario: Handle plugin with no parameterless constructor
    Given a plugin class has a constructor requiring parameters
    When Activator.CreateInstance attempts to instantiate the plugin
    Then a MissingMethodException should be thrown                                     # UAC083
    And the error should indicate no parameterless constructor found                   # UAC084
    And the plugin should fail to load                                                 # UAC085

  @SC28 @EdgeCase
  Scenario: Handle empty Assemblies collection
    Given a plugin has an empty Assemblies collection
    When the plugin needs to reference its assemblies
    Then the collection should be safely enumerable                                    # UAC086
    And no null reference exceptions should occur                                      # UAC087
    And the plugin should handle the empty collection gracefully                       # UAC088

  @SC29 @EdgeCase
  Scenario: Handle plugin unregistration
    Given a plugin is registered and configured
    When an attempt is made to unregister or remove the plugin at runtime
    Then the framework behavior should be documented                                   # UAC089
    And proper cleanup should occur if plugin removal is supported                     # UAC090

  @SC30 @EdgeCase
  Scenario: Handle plugin logging when ILogger is not available
    Given the plugin framework attempts to log messages
    And ILogger is not configured in the DI container
    When logging is attempted
    Then a NullLogger should be used as fallback                                       # UAC091
    And the logger resolution should fail gracefully                                   # UAC092
    And the application should not crash due to missing logger                         # UAC093

  @SC31 @EdgeCase
  Scenario: Handle async exceptions in ConfigureContext
    Given a plugin ConfigureContext method has an async operation that fails
    When the Task is awaited
    Then the exception should be properly propagated                                   # UAC094
    And AggregateException should be unwrapped if present                              # UAC095
    And the original exception should be logged                                        # UAC096

  @SC32 @EdgeCase
  Scenario: Handle plugin metadata with special characters
    Given a plugin has metadata with special characters:
      | Property    | Value                                    |
      | Name        | My<Plugin>& "Special" 'Chars'            |
      | Description | Line1\nLine2\tTab                        |
    When the plugin metadata is stored or serialized
    Then special characters should be properly escaped                                 # UAC097
    And JSON serialization should handle the characters correctly                      # UAC098
    And no injection vulnerabilities should exist                                      # UAC099

  @SC33 @EdgeCase
  Scenario: Handle very large number of plugins
    Given 1000 plugins are configured
    When all plugins are loaded and configured
    Then the framework should handle the load                                          # UAC100
    And performance should remain acceptable                                           # UAC101
    And memory usage should be monitored                                               # UAC102
    And no overflow or stack issues should occur                                       # UAC103

  @SC34 @EdgeCase
  Scenario: Handle plugin assembly in non-standard location
    Given a plugin assembly is in a custom directory outside the application folder
    When the assembly path is specified
    Then the framework should support absolute paths                                   # UAC104
    And the assembly should load from the custom location                              # UAC105
    And security considerations should be documented                                   # UAC106

  @SC35 @EdgeCase
  Scenario: Handle plugin throwing exception from property getter
    Given a plugin property getter throws an exception
    When the property is accessed during plugin processing
    Then the exception should be caught and logged                                     # UAC107
    And the plugin should be skipped or default value used                             # UAC108
    And the application should remain stable                                           # UAC109
