using System.Linq;
using System.Collections.Generic;
using System.Threading;

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

    RunHealthCheckTests(results);

    RunE2ETests(results);

    // Write failed tests to console
    var success = !results.Any(r => r.Status == TestStatus.Failed);
    if (!success)
    {
      ConsoleHelper.WriteLineError($"The following test(s) failed:");
      foreach (var result in results.Where(r => r.Status == TestStatus.Failed))
      {
        ConsoleHelper.WriteLineError($"- Name: {result.Name}, Actual/Error: {result.FailCause}");
      }
    }

    return success;
  }

  private void RunHealthCheckTests(List<TestResult> results)
  {
    foreach (var healthTest in _configuration.Tests.HealthTests)
    {
      var executor = new HealthCheckExecutor(healthTest);
      var result = executor.ExecuteAsync(_configuration.Domain, CancellationToken.None)
        .GetAwaiter()
        .GetResult();
      results.Add(result);
    }
  }

  private void RunE2ETests(List<TestResult> results)
  {
    foreach (var e2eTest in _configuration.Tests.E2ETests)
    {
      var executor = new PlaywrightExecutor(e2eTest, _configuration.Headless, _configuration.Slow);
      var result = executor.ExecuteAsync(_configuration.Domain, CancellationToken.None)
        .GetAwaiter()
        .GetResult();
      results.Add(result);
    }
  }
}