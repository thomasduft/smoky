namespace tomware.Smoky;

internal class SmokyConfiguration
{
  /// <summary>
  /// Domain against the smoke tests need to run.
  /// </summary>
  public string Domain { get; set; } = string.Empty;

  /// <summary>
  /// For displaying the E2E in the actual browser (defaults to true).
  /// </summary>
  public bool Headless { get; set; } = true;

  /// <summary>
  /// For executing the E2E tests slowly (defaults to false).
  /// </summary>
  public bool Slow { get; set; } = false;

  /// <summary>
  /// Request timeout for executing the E2E tests (defaults to 30000 = 30s).
  /// </summary>
  public int Timeout { get; set; } = 30000;

  public Tests Tests { get; set; } = new Tests();
}