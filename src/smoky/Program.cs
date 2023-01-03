using McMaster.Extensions.CommandLineUtils;

using static tomware.Smoky.ConsoleHelper;

namespace tomware.Smoky;

class Program
{
  private const string HelpOption = "-?|-h|--help";

  static void Main(string[] args)
  {
    var app = new CommandLineApplication
    {
      Name = "smoky"
    };

    app.HelpOption(HelpOption);

    app.Command("ping", (command) =>
    {
      command.Description = "Executes a ping to a domain to test (i.e. ping \"https://my-domain.com\").";
      var domainArgument = command.Argument("domain", "Domain to ping");
      command.HelpOption(HelpOption);
      command.OnExecute(() =>
      {
        Pinger pinger = new Pinger(domainArgument.Value);
        return pinger.Ping() ? 0 : 1;
      });
    });

    app.Command("test", (command) =>
    {
      command.Description = "Executes the configured test assertions in the config tool (i.e. test config.json -d \"https://my-domain.com\").";
      var configArgument = command.Argument("config", "Configuration file for the run");
      var domainOption = command.Option("-d|--domain", "Domain to run the tests against.", CommandOptionType.SingleValue);
      command.HelpOption(HelpOption);
      command.OnExecute(() =>
      {
       // if it fails return 1 otherwise 0

        return 0;
      });
    });

    app.OnExecute(() =>
    {
      app.ShowHelp();

      return 0;
    });

    app.Execute(args);

    WriteLineSuccess("done...");
  }
}