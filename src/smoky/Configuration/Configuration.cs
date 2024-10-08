using System.Text.Json.Serialization;

using Newtonsoft.Json.Converters;

namespace tomware.Smoky;

internal class SmokyConfiguration
{
  /// <summary>
  /// Domain against the smoke tests need to run.
  /// </summary>
  public string Domain { get; set; } = string.Empty;

  /// <summary>
  /// Enables video recording for all pages into the specified directory. 
  /// If not specified videos are not recorded. 
  /// </summary>
  /// <value></value>
  public string RecordVideoDir { get; set; } = string.Empty;

  /// <summary>
  /// For displaying the E2E in the actual browser (defaults to true).
  /// </summary>
  public bool Headless { get; set; } = true;

  /// <summary>
  /// For executing the E2E tests slowly (defaults to false).
  /// </summary>
  public int? Slow { get; set; }

  /// <summary>
  /// Request timeout for executing the E2E tests (defaults to 30000 = 30s).
  /// </summary>
  public int Timeout { get; set; } = 30000;

  /// <summary>
  /// Browser types
  /// Supported values are: chrome, firefox, webkit
  /// </summary>
  [JsonConverter(typeof(StringEnumConverter))]
  public BrowserType BrowserType { get; set; } = BrowserType.Chrome;

  public Tests Tests { get; set; } = new Tests();
}

internal enum BrowserType
{
  Chrome,
  Firefox,
  Webkit
}