using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace tomware.Smoky;

internal class PlaywrightExecutor : ITestExcecutor
{
  private readonly E2ETest _config;
  private readonly bool _headless;
  private readonly bool _slow;

  public PlaywrightExecutor(E2ETest config, bool headless, bool slow)
  {
    _config = config;
    _headless = headless;
    _slow = slow;
  }

  public async Task<TestResult> ExecuteAsync(string domain, CancellationToken cancellationToken)
  {
    try
    {
      // TODO: instruct Playwright
      // see:
      // - api reference: https://playwright.dev/dotnet/docs/api/class-playwright
      // - https://github.com/microsoft/playwright-dotnet
      // - https://www.youtube.com/watch?v=DbtP9kSbw5s

      using var playwright = await Playwright.CreateAsync();
      await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
      {
        Headless = _headless,
        SlowMo = _slow ? 100 : null // by N milliseconds per operation,
      });

      var context = await browser.NewContextAsync(new BrowserNewContextOptions
      {
        IgnoreHTTPSErrors = true
      });

      var page = await context.NewPageAsync();
      var url = $"{domain}/{_config.Route}";

      await page.GotoAsync(url);

      foreach (var assertion in _config.Assertions)
      {
        var value = await page.Locator(assertion.Selector).InnerHTMLAsync();
        if (value != assertion.Expected)
        {
          return TestResult.Failed(assertion.Name, value);
        }
        
        // await Assertions
        //   .Expect(page.Locator(assertion.Selector))
        //   .ToHaveValueAsync(assertion.Expected);
      }

      return TestResult.Passed(_config.Name);
    }
    catch (Exception ex)
    {
      return TestResult.FailedWithOtherReason(_config.Name, ex.Message);
    }
  }
}
