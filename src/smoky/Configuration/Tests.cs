using System.Collections.Generic;

namespace tomware.Smoky;

internal class Tests
{
  public List<HealthTest> HealthTests { get; set; } = new List<HealthTest>();
  public List<E2ETest> E2ETests { get; set; } = new List<E2ETest>();
}