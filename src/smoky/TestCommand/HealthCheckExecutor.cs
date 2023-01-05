using Newtonsoft.Json.Linq;

namespace tomware.Smoky;

internal class HealthCheckExecutor
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

  public async Task<TestResult> ExecuteAsync(
    string domain,
    CancellationToken cancellationToken
  )
  {
    // TODO: provide health endpoint as configuration
    var url = $"{domain}/health";

    var propertyPath = _config.PropertyPath;
    if (string.IsNullOrWhiteSpace(propertyPath))
    {
      return TestResult.FailedWithOtherReason(
        _config.Name,
        $"Please configure a non-empty PropertyPath"
      );
    };

    try
    {
      var response = await _client.GetAsync(url);
      if (!response.IsSuccessStatusCode)
      {
        return TestResult.FailedWithOtherReason(
          _config.Name,
          response.ReasonPhrase ?? $"Could not get any valid response from url '{url}'"
        );
      }

      var json = await response.Content.ReadAsStringAsync();

      JObject o = JObject.Parse(json);
      string value = (string)o.SelectToken(propertyPath)!;

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
