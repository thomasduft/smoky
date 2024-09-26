using McMaster.Extensions.CommandLineUtils;

namespace tomware.Smoky;

public class PingCommand : CommandLineApplication
{
  private readonly CommandArgument<string> _domainArgument;

  public PingCommand()
  {
    Name = "ping";
    Description = "Executes a ping to a domain to test (eg. ping \"https://my-domain.com\").";

    _domainArgument = Argument<string>(
      "domain",
      "Domain to ping",
      cfg => cfg.IsRequired(),
      true
    );

    OnExecuteAsync(ExecuteAsync);
  }

  private async Task<int> ExecuteAsync(CancellationToken cancellationToken)
  {
    Pinger pinger = new(_domainArgument.Value!);
    return await pinger.PingAsync(cancellationToken)
      ? 0
      : 1;
  }
}