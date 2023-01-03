using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace tomware.Smoky;

internal class HealthCheckExecutor : ITestExcecutor
{
  private readonly HealthTest _config;
  private readonly HttpClient _client;

  public HealthCheckExecutor(HealthTest config)
  {
    _config = config;

    // below HttpClientHanlder disables self signed certificates issues
    _client = new HttpClient(new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = (s, ce, ca, p) => true
    });
  }

  public TestResult Execute(string domain)
  {
    var url = $"{domain}/health";

    try
    {
      var response = _client.GetAsync(url).GetAwaiter().GetResult();
      if (!response.IsSuccessStatusCode)
      {
        return TestResult.FailedWithOtherReason(_config.Name, response.ReasonPhrase);
      }

      var json = response.Content.ReadAsStringAsync()
        .GetAwaiter()
        .GetResult();

      JObject o = JObject.Parse(json);
      var value = (string)o.SelectToken(_config.PropertyPath);
      
      // assert
      if (value == _config.Expected)
      {
        return TestResult.Passed(_config.Name);
      }

      return TestResult.Failed(_config.Name, value);
    }
    catch (Exception ex)
    {
      return TestResult.FailedWithOtherReason(_config.Name, ex.Message);
    }
  }
}
