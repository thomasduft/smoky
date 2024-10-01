namespace tomware.Smoky;

internal class Initializer
{
  private readonly string _name;

  public Initializer(string name)
  {
    _name = name;
  }

  public async Task<bool> InitAsync(CancellationToken cancellationToken)
  {
    var config = new SmokyConfiguration
    {
      Domain = "<your-domain-here>",
      RecordVideoDir = "",
      Headless = false,
      Slow = 200,
      Timeout = 30000,
      BrowserType = BrowserType.Chrome,
      Tests = new Tests()
    };

    await File.WriteAllTextAsync(
        $"{_name}.json",
        config.ToJsonForInitCommand(),
        cancellationToken
    );

    // also paste smoky-schema.json next to the config file
    var jsonSchema = ResourceLoader.GetResource(Resources.SmokyJsonSchema);
    await File.WriteAllTextAsync(
        $"{_name}-schema.json",
        jsonSchema,
        cancellationToken
    );

    return true;
  }
}