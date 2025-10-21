# --------------------------------------------------------------------
# Feature: Database Provider Support
# --------------------------------------------------------------------

@VCHIP-0010 @UC05 @Plugin @Database
Feature: Database Provider Support
  As a developer using the LowlandTech Plugin Framework
  I want plugins to support multiple database providers
  So that I can configure data access based on my application needs

  Background:
    Given the Providers enum is available
    And plugins can be configured with different database providers

  @SC01
  Scenario: Providers enum contains all supported databases
    Given the Providers enum is defined
    Then it should contain the following values:                                       # UAC001
      | Provider   |
      | InMemory   |
      | Sqlite     |
      | PgSql      |
      | SqlServer  |
      | MySql      |

  @SC02
  Scenario: Use InMemory provider for testing
    Given a plugin needs database access
    And the environment is set to Testing
    When the plugin is configured with Providers.InMemory
    Then the plugin should use an in-memory database                                   # UAC002
    And no persistent storage should be required                                       # UAC003
    And the database should be cleared between test runs                               # UAC004

  @SC03
  Scenario: Use SQLite provider for lightweight applications
    Given a plugin needs database access
    And the application requires file-based storage
    When the plugin is configured with Providers.Sqlite
    Then the plugin should use SQLite database                                         # UAC005
    And a database file should be created                                              # UAC006
    And the plugin should support local file-based storage                             # UAC007

  @SC04
  Scenario: Use PostgreSQL provider for production
    Given a plugin needs database access
    And the application uses PostgreSQL in production
    When the plugin is configured with Providers.PgSql
    Then the plugin should use PostgreSQL database                                     # UAC008
    And the plugin should connect using PostgreSQL connection string                   # UAC009
    And PostgreSQL-specific features should be available                               # UAC010

  @SC05
  Scenario: Use SQL Server provider for enterprise applications
    Given a plugin needs database access
    And the application uses Microsoft SQL Server
    When the plugin is configured with Providers.SqlServer
    Then the plugin should use SQL Server database                                     # UAC011
    And the plugin should connect using SQL Server connection string                   # UAC012
    And SQL Server-specific features should be available                               # UAC013

  @SC06
  Scenario: Use MySQL provider for web applications
    Given a plugin needs database access
    And the application uses MySQL database
    When the plugin is configured with Providers.MySql
    Then the plugin should use MySQL database                                          # UAC014
    And the plugin should connect using MySQL connection string                        # UAC015
    And MySQL-specific features should be available                                    # UAC016

  @SC07
  Scenario: Plugin registers DbContext with selected provider
    Given a plugin has a DbContext that needs configuration
    And the plugin should support multiple providers
    When the plugin Install method is called
    Then the plugin should read the provider configuration                             # UAC017
    And the plugin should register the DbContext with the appropriate provider         # UAC018
    And the DbContext should be available through dependency injection                 # UAC019

  @SC08 @Configuration
  Scenario: Configure provider through appsettings.json
    Given the appsettings.json contains database configuration:
      """
      {
        "Database": {
          "Provider": "Sqlite",
          "ConnectionString": "Data Source=app.db"
        }
      }
      """
    When a plugin reads the database configuration
    Then the plugin should use Providers.Sqlite                                        # UAC020
    And the plugin should use the specified connection string                          # UAC021

  @SC09
  Scenario: Environment-specific provider configuration
    Given the application runs in Development environment
    And appsettings.Development.json specifies Providers.InMemory
    When the plugin configures its database
    Then the plugin should use InMemory provider for Development                       # UAC022
    And no production database connection should be required                           # UAC023

  @SC10
  Scenario: Production environment uses persistent database
    Given the application runs in Production environment
    And appsettings.Production.json specifies Providers.PgSql
    When the plugin configures its database
    Then the plugin should use PostgreSQL provider                                     # UAC024
    And data should be persisted to the production database                            # UAC025

  @SC11
  Scenario: Switch providers without code changes
    Given a plugin is configured with Providers.Sqlite
    When the configuration is changed to Providers.PgSql
    And the application is restarted
    Then the plugin should use PostgreSQL instead of SQLite                            # UAC026
    And no code changes should be required                                             # UAC027

  @SC12
  Scenario: Plugin handles provider-specific migrations
    Given a plugin uses Entity Framework Core
    And the database provider is configured
    When database migrations are applied
    Then the migrations should be compatible with the selected provider                # UAC028
    And provider-specific SQL should be generated correctly                            # UAC029

  @SC13
  Scenario: Provider enum can be serialized to configuration
    Given a Providers enum value of Providers.PgSql
    When the value is serialized to JSON configuration
    Then it should serialize as "PgSql"                                                # UAC030
    And it should deserialize back to Providers.PgSql                                  # UAC031

  @SC14 @ErrorHandling
  Scenario: Invalid provider configuration handling
    Given the configuration specifies an invalid provider name
    When the plugin attempts to parse the provider
    Then an error should be thrown or a default provider should be used                # UAC032
    And the error should clearly indicate the invalid configuration                    # UAC033

  @SC15
  Scenario: Plugin with multiple DbContexts using different providers
    Given a plugin has ReadDbContext and WriteDbContext
    When the plugin is configured
    Then ReadDbContext might use Providers.Sqlite for read replicas                    # UAC034
    And WriteDbContext might use Providers.PgSql for primary database                  # UAC035
    And both contexts should be registered independently                               # UAC036

  @SC16
  Scenario: Provider-agnostic plugin code
    Given a plugin is written to be provider-agnostic
    When the plugin uses standard Entity Framework Core APIs
    Then the plugin code should work with any Providers enum value                     # UAC037
    And provider-specific code should be minimal or abstracted                         # UAC038

  @SC17
  Scenario: Test plugin with InMemory provider
    Given an integration test for a plugin
    When the test configures the plugin with Providers.InMemory
    Then the test should run without requiring database infrastructure                 # UAC039
    And the test should execute quickly                                                # UAC040
    And test data should be isolated between test runs                                 # UAC041

  @SC18 @Validation
  Scenario: Connection string validation for each provider
    Given a plugin is configured with a specific provider
    When the plugin validates the connection string
    Then the connection string should be valid for the selected provider               # UAC042
    And provider-specific connection string requirements should be checked             # UAC043

  @SC19
  Scenario: Provider-specific features are utilized
    Given a plugin uses PostgreSQL-specific features like JSONB
    And the provider is set to Providers.PgSql
    When the plugin configures the DbContext
    Then PostgreSQL-specific mappings should be applied                                # UAC044
    And the features should work correctly                                             # UAC045

  @SC20 @EdgeCase
  Scenario: Fallback to default provider
    Given no provider is specified in configuration
    When a plugin needs to configure a database
    Then the plugin should use a default provider (e.g., InMemory or Sqlite)           # UAC046
    And the default should be documented                                               # UAC047
    And the application should log which default was used                              # UAC048

  @SC21
  Scenario: Provider impacts DbContext configuration
    Given a plugin configures a DbContext
    When the provider is Providers.InMemory
    Then UseInMemoryDatabase should be called on DbContextOptionsBuilder              # UAC049
    When the provider is Providers.Sqlite
    Then UseSqlite should be called with the connection string                         # UAC050
    When the provider is Providers.PgSql
    Then UseNpgsql should be called with the connection string                         # UAC051
    When the provider is Providers.SqlServer
    Then UseSqlServer should be called with the connection string                      # UAC052
    When the provider is Providers.MySql
    Then UseMySql should be called with the connection string                          # UAC053
