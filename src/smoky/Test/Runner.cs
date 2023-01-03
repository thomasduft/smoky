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

    // 

    //

    var success = !results.Any(r => r.Status == TestStatus.Failed);
    if (!success)
    {
      // Echo all failed results
      foreach (var result in results.Where(r => r.Status == TestStatus.Failed))
      {
        ConsoleHelper.WriteLineError($"Name: '{result.Name}', Actual value: '{result.Actual}'");
      }
    }

    return success;
  }
}