using System.Linq;
using System.Collections.Generic;

namespace tomware.Smoky;

internal class Runner
{
  private readonly SmokyConfiguration _configuration;
  private readonly string _domain;

  public Runner(SmokyConfiguration configuration, string domain)
  {
    _configuration = configuration;
    _domain = domain;
  }

  public bool Run()
  {
    var results = new List<TestResult>();

    // HealthChecks
    foreach (var healthTest in _configuration.Tests.HealthTests)
    {
      var executor = new HealthCheckExecutor(healthTest);
      var result = executor.Execute(_configuration.Domain);
      results.Add(result);
    }

    // E2ETests
    // TODO

    var success = !results.Any(r => r.Status == TestStatus.Failed);
    if (!success)
    {
      // Echo all failed results
      foreach (var result in results.Where(r => r.Status == TestStatus.Failed))
      {
        ConsoleHelper.WriteLineError($"Name: '{result.Name}', Cause: '{result.FailCause}'");
      }
    }

    return success;
  }
}