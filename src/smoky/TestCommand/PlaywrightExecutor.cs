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
      await page.GotoAsync(requestUri);
      
      // being safe and try again in certain case :-)
      if (!page.Url.StartsWith(requestUri))
      {
        await page.GotoAsync(requestUri);
      }

      // Arrange
      if (config.Arrange is not null && config.Arrange.Any())
      {
        foreach (var arrange in config.Arrange)
        {
          await page.Locator(arrange.Selector).FillAsync(arrange.Input);
          await Assertions
            .Expect(page.Locator(arrange.Selector))
            .ToHaveValueAsync(arrange.Input);
        }
      }

      // Act
      if (config.Act is not null)
      {
        await page.Locator(config.Act.Selector).ClickAsync();
      }

      // Assert
      if (config.Assert is not null && config.Assert.Any())
      {
        foreach (var assert in config.Assert)
        {
          // await page.Locator(assert.Selector).WaitForAsync();
          var value = await page.Locator(assert.Selector).InnerHTMLAsync();
          if (value != assert.Expected)
          {
            return TestResult.Failed(assert.Name, value);
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

  private static string GetStoragePath()
  {
    var path = Path.Combine(Environment.CurrentDirectory, "state.json");
    return path;
  }
}
