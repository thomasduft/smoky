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

    foreach (var test in tests)
    {
      var result = await TestAsync(browser, domain, test, cancellationToken);

      // // store the state if signed in if then required since I use always the same page object!
      // if (test.Act is not null && test.Act.IsLogin)
      // {
      //   // Automate loggin in
      //   // see: https://playwright.dev/dotnet/docs/auth#automate-logging-in
      //   var path = await context.StorageStateAsync(new BrowserContextStorageStateOptions
      //   {
      //     Path = GetStoragePath(),
      //   });

      //   // Get session storage and store as env variable
      //   var sessionStorage = await page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
      //   Environment.SetEnvironmentVariable("SESSION_STORAGE", sessionStorage);

      //   // Set session storage in a new context
      //   var loadedSessionStorage = Environment.GetEnvironmentVariable("SESSION_STORAGE");
      //   await context.AddInitScriptAsync(@"(storage => {
      //       if (window.location.hostname === 'example.com') {
      //         const entries = JSON.parse(storage);
      //         for (const [key, value] of Object.entries(entries)) {
      //           window.sessionStorage.setItem(key, value);
      //         }
      //       }
      //     })('" + loadedSessionStorage + "')");

      //   _signedIn = true;
      // }

      results.Add(result);
    }

    return results;
  }

  private async Task<TestResult> TestAsync(
    IBrowser browser,
    string domain,
    E2ETest config,
    CancellationToken cancellationToken
  )
  {
    try
    {
      var context = await GetBrowserContext(browser);
      var page = await context.NewPageAsync();

      var url = $"{domain}/{config.Route}";
      await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
      if (!page.Url.StartsWith(url))
      {
        await page.GotoAsync(url);
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

    // if (_signedIn)
    // {
    //   try
    //   {
    //     context = await browser.NewContextAsync(new BrowserNewContextOptions
    //     {
    //       IgnoreHTTPSErrors = true,
    //       StorageState = GetStoragePath()
    //     });
    //   }
    //   catch (Exception ex)
    //   {
    //     ConsoleHelper.WriteLineError($"{ex.Message}");
    //   }
    // }
    // else
    // {
    //   context = await browser.NewContextAsync(new BrowserNewContextOptions
    //   {
    //     IgnoreHTTPSErrors = true,
    //   });
    // }

    context.SetDefaultTimeout(5000);

    return context;
  }

  private static string GetStoragePath()
  {
    var path = Path.Combine(Environment.CurrentDirectory, "state.json");
    return path;
  }
}
