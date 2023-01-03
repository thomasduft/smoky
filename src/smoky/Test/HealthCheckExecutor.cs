using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

  public async Task<TestResult> ExecuteAsync(string domain, CancellationToken cancellationToken)
  {
    // TODO: provide health endpoint as configuration
    var url = $"{domain}/health";

    try
    {
      var response = await _client.GetAsync(url);
      if (!response.IsSuccessStatusCode)
      {
        return TestResult.FailedWithOtherReason(_config.Name, response.ReasonPhrase);
      }

      var json = await response.Content.ReadAsStringAsync();

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
