using Microsoft.Extensions.DependencyInjection;

using tomware.Smoky;

var services = new ServiceCollection()
    .AddCliCommand<InitCommand>()
    .AddCliCommand<PingCommand>()
    .AddCliCommand<TestCommand>()
    .AddSingleton<Cli>();

var provider = services.BuildServiceProvider();
var cli = provider.GetRequiredService<Cli>();
cli.Name = "smoky";
cli.Description = "A simple smoke test tool";

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
  Console.WriteLine("Cancelling...");
  cts.Cancel();
  e.Cancel = true;
};

return await cli.ExecuteAsync(args, cts.Token);