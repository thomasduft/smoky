using Newtonsoft.Json.Linq;

namespace tomware.Smoky;

internal class HealthCheckExecutor
{
  private readonly HealthTest _config;
  private readonly HttpClient _client;

  public HealthCheckExecutor(HealthTest config)
  {
    _config = config;

    // below HttpClientHandler disables self signed certificates issues
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
    var requestUri = $"{domain}/{_config.Route}";

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
      var response = await _client.GetAsync(requestUri);
      if (!response.IsSuccessStatusCode)
      {
        // Question: How to deal with proxies?

        return TestResult.FailedWithOtherReason(
          _config.Name,
          response.ReasonPhrase ?? $"Could not get any valid response from url '{requestUri}'"
        );
      }

      var json = await response.Content.ReadAsStringAsync();

      JObject o = JObject.Parse(json);
      string value = (string)o.SelectToken(propertyPath)!;

      // assert
      return value == _config.Expected
        ? TestResult.Passed(_config.Name)
        : TestResult.Failed(_config.Name, "n/a", value);
    }
    catch (Exception ex)
    {
      return TestResult.FailedWithOtherReason(_config.Name, ex.Message);
    }
  }
}