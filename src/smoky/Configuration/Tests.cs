using System.Collections.Generic;

namespace tomware.Smoky;

internal class Tests
{
  public List<HealthTest> HealthTests { get; set; } = new List<HealthTest>();
  public List<object> E2ETests { get; set; } = new List<object>();
}