using System.Collections.Generic;

namespace tomware.Smoky;

internal class E2ETest
{
  public string Name { get; set; } = string.Empty;
  public string Route { get; set; } = string.Empty;
  public List<E2EAssertion> Assertions { get; set; } = new List<E2EAssertion>();
  public List<E2ECommand> Commands { get; set; } = new List<E2ECommand>();
}

internal class E2EAssertion
{
  public string Name { get; set; } = string.Empty;
  public string Selector { get; set; } = string.Empty;
  public string Expected { get; set; } = string.Empty;
}

internal class E2ECommand
{
  public string Name { get; set; } = string.Empty;
  public string Input { get; set; } = string.Empty;
  public string Selector { get; set; } = string.Empty;
}
