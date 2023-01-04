using McMaster.Extensions.CommandLineUtils;

using tomware.Smoky;

using static tomware.Smoky.ConfigurationHelper;
using static tomware.Smoky.ConsoleHelper;

var app = new CommandLineApplication
{
  Name = "smoky"
};
const string HelpOption = "-?|-h|--help";

app.HelpOption(HelpOption);

app.Command("ping", (command) =>
{
  command.Description = "Executes a ping to a domain to test (eg. ping \"https://my-domain.com\").";
  var domainArgument = command.Argument("domain", "Domain to ping");
  command.HelpOption();
  command.OnExecuteAsync(async cancellationToken =>
  {
    if (!domainArgument.HasValue)
    {
      Exit($"Please specify a valid domain argument");
    }

    Pinger pinger = new Pinger(domainArgument.Value!);
    return await pinger.Ping(cancellationToken)
      ? 0
      : 1;
  });
});

app.Command("test", (command) =>
{
  command.Description = "Executes the configured test assertions in the config tool (eg. test config.json -d \"https://my-domain.com\").";
  var configArgument = command.Argument("config", "Configuration file for the run");
  var domainOption = command.Option("-d|--domain", "Domain to run the tests against.", CommandOptionType.SingleValue);
  command.HelpOption();
  command.OnExecuteAsync(async cancellationToken =>
  {
    if (!configArgument.HasValue)
    {
      Exit($"Please specify a valid configArgument argument");
    }

    var config = GetConfiguration(configArgument.Value!);
    var domain = domainOption.HasValue()
      ? domainOption.Value()
      : config.Domain;

    if (string.IsNullOrWhiteSpace(domain))
    {
      Exit($"Please specify a valid domain option!");
    }

    // further optional args for testing purposes
    // -h|--headless - for displaying the E2E in the actual browser
    // -s|--slow - for executing the E2E tests slowly

    // if it fails return 1 otherwise 0
    Runner runner = new Runner(config, domain!);
    return await runner.Run(cancellationToken)
      ? 0
      : 1;
  });
});

app.OnExecute(() =>
{
  app.ShowHelp();

  return 0;
});

return app.Execute(args);
