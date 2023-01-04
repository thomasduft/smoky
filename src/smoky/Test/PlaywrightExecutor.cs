using Microsoft.Playwright;

namespace tomware.Smoky;

// TODO: instruct Playwright
// see:
// - api reference: https://playwright.dev/dotnet/docs/api/class-playwright
// - https://github.com/microsoft/playwright-dotnet
// - https://www.youtube.com/watch?v=DbtP9kSbw5s
internal class PlaywrightExecutor
{
  private const string StorageStatePath = "state.json";

  private bool _signedIn = false;

  private readonly bool _headless;
  private readonly bool _slow;

  public PlaywrightExecutor(bool headless, bool slow)
  {
    _headless = headless;
    _slow = slow;
  }

  public async Task<IEnumerable<TestResult>> ExecuteAsync(
    string domain,
    IEnumerable<E2ETest> tests,
    CancellationToken cancellationToken
  )
  {
    var results = new List<TestResult>();

    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = _headless,
      SlowMo = _slow ? 100 : null // by N milliseconds per operation,
    });
    var context = await GetBrowserContext(browser);

    foreach (var test in tests)
    {
      var result = await TestAsync(context, domain, test, cancellationToken);
      results.Add(result);
    }

    return results;
  }

  private async Task<TestResult> TestAsync(
    IBrowserContext context,
    string domain,
    E2ETest config,
    CancellationToken cancellationToken
  )
  {
    try
    {
      var page = await context.NewPageAsync();
      var url = $"{domain}/{config.Route}";
      await page.GotoAsync(url);

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

        if (config.Act.IsLogin)
        {
          var path = await context.StorageStateAsync(new BrowserContextStorageStateOptions
          {
            Path = StorageStatePath,
          });

          _signedIn = true;
        }
      }

      // Assert
      if (config.Assert is not null && config.Assert.Any())
      {
        foreach (var assert in config.Assert)
        {
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

    if (_signedIn)
    {
      try
      {
        context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
          IgnoreHTTPSErrors = true,
          StorageState = StorageStatePath
        });
      }
      catch (Exception ex)
      {
        ConsoleHelper.WriteLineError($"{ex.Message}");
      }
    }
    else
    {
      context = await browser.NewContextAsync(new BrowserNewContextOptions
      {
        IgnoreHTTPSErrors = true,
      });
    }

    return context;
  }
}
