namespace tomware.Smoky;

internal class Initializer
{
  private readonly string _name;

  public Initializer(string name)
  {
    _name = name;
  }

  public async Task<bool> Init(CancellationToken cancellationToken)
  {
    var config = new SmokyConfiguration
    {
      Domain = "<your-domain-here>",
      RecordVideoDir = "",
      Headless = false,
      Slow = 200,
      Timeout = 30000,
      Channel = "chrome",
      Tests = new Tests()
    };

    await File.WriteAllTextAsync(
        $"{_name}.json",
        config.ToJsonForInitCommand()
    );

    return true;
  }
}