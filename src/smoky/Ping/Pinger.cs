using System.Net.Http;

namespace tomware.Smoky;

internal class Pinger
{
  private readonly string _domain;
  private readonly HttpClient _client;

  public Pinger(string domain)
  {
    _domain = domain;
    _client = new HttpClient(new HttpClientHandler
    {
      ServerCertificateCustomValidationCallback = (s, ce, ca, p) => true,
      AllowAutoRedirect = false
    });
  }

  public bool Ping()
  {
    ConsoleHelper.WriteLineYellow($"Trying contact domain '{_domain}'");
    bool success = false;

    try
    {
      _client.GetAsync(_domain).GetAwaiter().GetResult();
      // usually check sth. like this = response.IsSuccessStatusCode;
      // but since we get an Unauthorized HttpStatus we just say as 
      // long as we do not get an exceptions somehow the domain was
      // in reach!

      success = true;
    }
    catch (HttpRequestException ex)
    {
      ConsoleHelper.WriteLineError($"Exception: {ex.Message} - {ex.StackTrace}");
    }

    return success;
  }
}