using McMaster.Extensions.CommandLineUtils;

namespace tomware.Smoky;

public class InitCommand : CommandLineApplication
{
  private readonly CommandOption<string> _nameOption;

  public InitCommand()
  {
    Name = "init";
    Description = "Initializes and scaffolds an empty smoky config file (eg. init -n <some-name>).";

    _nameOption = Option<string>(
      "-n|--name",
      "Name of the smoky config file (defaults to 'smoky').",
      CommandOptionType.SingleValue,
      cfg => cfg.DefaultValue = "smoky",
      true
    );

    OnExecuteAsync(ExecuteAsync);
  }

  private async Task<int> ExecuteAsync(CancellationToken cancellationToken)
  {
    Initializer initializer = new(_nameOption.Value()!);

    return await initializer.InitAsync(cancellationToken)
      ? 0
      : 1;
  }
}