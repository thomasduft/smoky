# Smoky

`smoky` - a simple smoke test tool.

## Idea

`smoky` is a CLI-based tool capable of orchestrating simple smoke tests against a running web application.

Using a configuration file, you can easily configure acceptance smoke tests. The currently supported types are:

- HealthCheck tests: These tests make it possible to check the health of a web application
- E2E tests: Leveraging [Playwright for .NET](https://playwright.dev/dotnet/) these tests allow simple E2E tests to be run using a headless browser (currently Chrome-based only)

> The basic intent of `smoky` is to perform some smoke tests on a web application during the integration deployment pipeline and get instant feedback on whether the deployment succeeded and the web application is up and running as expected.

## Usage

```console
Usage: smoky [command] [options]

Options:
  -?|-h|--help  Show help information.

Commands:
  ping          Executes a ping to a domain to test (eg. ping "https://my-domain.com").
  test          Executes the configured test assertions in the config tool (eg. test config.json -d "https://my-domain.com").

Run 'smoky [command] -?|-h|--help' for more information about a command.
```

## Configuration sample

```json
{
  "Domain": "https://localhost:5001",
  "Headless": true,
  "Slow": false,
  "Timeout": 20000,
  "Tests": {
    "HealthTests": [
      {
        "Name": "System is healthy",
        "Expected": "Healthy",
        "PropertyPath": "status"
      },
      {
        "Name": "All users seeded",
        "Expected": "Healthy",
        "PropertyPath": "info[0].status"
      }
    ],
    "E2ETests": [
      {
        "Name": "Should login",
        "Route": "Login",
        "Arrange": [
          {
            "Name": "Should enter username",
            "Selector": "input#Input_Username",
            "Input": "admin"
          },
          {
            "Name": "Should enter password",
            "Selector": "input#Input_Password",
            "Input": "password"
          }
        ],
        "Act": {
          "Name": "Should click login",
          "Selector": "button[type=submit]",
          "Click": true
        },
        "Assert": [
          {
            "Name": "Should display Logout button",
            "Selector": "button[type=submit]",
            "Expected": "Logout"
          }
        ]
      },
      {
        "Name": "Should display holiday page",
        "Route": "holiday",
        "Assert": [
          {
            "Name": "Should display title Holidays",
            "Selector": "h3",
            "Expected": "Holidays"
          }
        ]
      }
    ]
  }
}
```
