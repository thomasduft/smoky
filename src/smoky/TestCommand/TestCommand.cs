using McMaster.Extensions.CommandLineUtils;

namespace tomware.Smoky;

using static tomware.Smoky.ConsoleHelper;

public class TestCommand : CommandLineApplication
{
  private readonly CommandArgument<string> _configArgument;
  private readonly CommandOption<string> _domainOption;

  public TestCommand()
  {
    Name = "test";
    Description = "Executes the configured test assertions in the config tool (eg. test config.json -d \"https://my-domain.com\").";

    _configArgument = Argument<string>(
      "config",
      "Configuration file for the run",
      cfg => cfg.IsRequired(),
      true
    );

    _domainOption = Option<string>(
      "-d|--domain",
      "Domain to run the tests against.",
      CommandOptionType.SingleValue,
      cfg => cfg.DefaultValue = null,
      true
    );

    OnExecuteAsync(ExecuteAsync);
  }

  private async Task<int> ExecuteAsync(CancellationToken cancellationToken)
  {
    var config = GetConfiguration(_configArgument.Value!);
    var domain = _domainOption.HasValue()
      ? _domainOption.Value()
      : config.Domain;

    Runner runner = new(config, domain!);
    return await runner.RunAsync(cancellationToken)
      ? 0
      : 1;
  }

  private static SmokyConfiguration GetConfiguration(string file)
  {
    SmokyConfiguration? configuration = null;
    try
    {
      WriteLine($"Reading config from file '{file}'");
      configuration = File.ReadAllText(file)
        .FromJson<SmokyConfiguration>();
    }
    catch (Exception ex)
    {
      Exit($"Error in reading config file! Exception: '{ex.Message}'");
    }

    return configuration!;
  }
}