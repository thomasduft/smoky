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

  public async Task<bool> RunAsync(CancellationToken cancellationToken)
  {
    ConsoleHelper.WriteLineYellow($"Starting smoke test execution for '{_configuration.Domain}' with '{_configuration.BrowserType}' browser...");

    var results = new List<TestResult>();

    if (_configuration.Tests.HealthTests.Any())
      await RunHealthCheckTests(results, cancellationToken);

    if (_configuration.Tests.E2ETests.Any())
      await RunE2ETests(results);

    // Write failed tests to console
    var success = !results.Any(r => r.Status == TestStatus.Failed);
    if (!success)
    {
      ConsoleHelper.WriteLineError($"The following test(s) failed:");
      foreach (var result in results.Where(r => r.Status == TestStatus.Failed))
      {
        ConsoleHelper.WriteLineError($"- Name: {result.Name}, Step: {result.TestStep}, Actual/Error: {result.FailCause}");
      }
    }
    else
    {
      ConsoleHelper.WriteLineSuccess($"'{results.Count}' tests successfully passed...");
    }

    return success;
  }

  private async Task RunHealthCheckTests(
    List<TestResult> results,
    CancellationToken cancellationToken
  )
  {
    foreach (var healthTest in _configuration.Tests.HealthTests)
    {
      var executor = new HealthCheckExecutor(healthTest);
      var result = await executor.ExecuteAsync(
        _domain,
        cancellationToken
      );
      results.Add(result);
    }
  }

  private async Task RunE2ETests(List<TestResult> results)
  {
    var executor = new PlaywrightExecutor(
      _configuration.Headless,
      _configuration.Slow,
      _configuration.Timeout,
      _configuration.BrowserType,
      _configuration.RecordVideoDir
    );
    var result = await executor.ExecuteAsync(
      _domain,
      _configuration.Tests.E2ETests
    );

    results.AddRange(result);
  }
}