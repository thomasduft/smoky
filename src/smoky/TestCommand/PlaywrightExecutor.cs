using Microsoft.Playwright;

namespace tomware.Smoky;

// TODO: instruct Playwright
// see:
// - api reference: https://playwright.dev/dotnet/docs/api/class-playwright
// - https://github.com/microsoft/playwright-dotnet
// - https://www.youtube.com/watch?v=DbtP9kSbw5s
internal class PlaywrightExecutor
{
  private readonly bool _headless;
  private readonly int? _slow;
  private readonly int _timeout;
  private readonly string _channel;

  private string _requestUri = string.Empty;

  public PlaywrightExecutor(bool headless, int? slow, int timeout, string channel)
  {
    _headless = headless;
    _slow = slow;
    _timeout = timeout;
    _channel = channel;
  }

  public async Task<IEnumerable<TestResult>> ExecuteAsync(
    string domain,
    IEnumerable<E2ETest> tests,
    CancellationToken cancellationToken
  )
  {
    var results = new List<TestResult>();

    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium
      .LaunchAsync(new BrowserTypeLaunchOptions
      {
        Channel = !string.IsNullOrWhiteSpace(_channel) ? _channel : null,
        Headless = _headless,
        SlowMo = _slow // by N milliseconds per operation,
      });

    var context = await GetBrowserContext(browser);
    var page = await context.NewPageAsync();

    foreach (var test in tests)
    {
      var result = await TestAsync(page, domain, test, cancellationToken);
      results.Add(result);
    }

    return results;
  }

  private async Task<TestResult> TestAsync(
    IPage page,
    string domain,
    E2ETest config,
    CancellationToken cancellationToken
  )
  {
    try
    {
      var requestUri = $"{domain}/{config.Route}";
      if (requestUri != _requestUri)
      {
        await page.GotoAsync(requestUri);
        _requestUri = requestUri;

        // being safe and try again in certain case :-)
        if (!page.Url.StartsWith(requestUri))
        {
          await page.GotoAsync(requestUri);
        }
      }

      // Arrange
      if (config.Arrange is not null && config.Arrange.Any())
      {
        foreach (var arrangeStep in config.Arrange)
        {
          await ProcessStepAsync(page, arrangeStep);
        }
      }

      // Act
      if (config.Act is not null)
      {
        await ProcessStepAsync(page, config.Act);
      }

      // Assert
      if (config.Assert is not null && config.Assert.Any())
      {
        foreach (var assertStep in config.Assert)
        {
          var value = await ProcessStepAsync(page, assertStep);
          if (!value)
          {
            return TestResult.Failed(assertStep.Step, "couldn't be located or visible");
          }
        }
      }

      return TestResult.Passed(config.Name);
    }
    catch (Exception ex)
    {
      return TestResult.FailedWithOtherReason(config.Name, ex.Message);
    }
  }

  private async Task<IBrowserContext> GetBrowserContext(IBrowser browser)
  {
    IBrowserContext? context = null;

    context = await browser.NewContextAsync(new BrowserNewContextOptions
    {
      IgnoreHTTPSErrors = true
    });

    context.SetDefaultTimeout(_timeout);

    return context;
  }

  private async Task<bool> ProcessStepAsync(IPage page, E2ETestStep step)
  {
    ILocator? locator = null;

    if (step.LocatorType == LocatorType.GetByLabel)
    {
      locator = page.GetByLabel(step.Text);
    }
    else if (step.LocatorType == LocatorType.GetByRole)
    {
      locator = page.GetByRole(step.AriaRole, new() { Name = step.Text });
    }
    else
    {
      locator = page.GetByText(step.Text);
    }

    if (step.Action == ActionType.Click)
    {
      await locator.ClickAsync();
    }
    else if (step.Action == ActionType.Fill)
    {
      await locator.FillAsync(step.Value);
    }
    else
    {
      return await locator.IsVisibleAsync();
    }

    return await Task.FromResult(true);
  }
}
