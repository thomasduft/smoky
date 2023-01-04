using System;
using System.IO;

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
        if (!domainArgument.HasValue)
        {
          Exit($"Please specify a valid domain argument");
        }

        Pinger pinger = new Pinger(domainArgument.Value!);
        return pinger.Ping()
          ? 0
          : 1;
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
        return runner.Run()
          ? 0
          : 1;
      });
    });

    app.OnExecute(() =>
    {
      app.ShowHelp();

      return 0;
    });

    app.Execute(args);
  }

  static SmokyConfiguration GetConfiguration(string file)
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