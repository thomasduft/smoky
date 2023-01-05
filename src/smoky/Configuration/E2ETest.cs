namespace tomware.Smoky;

internal class E2ETest
{
  public string Name { get; set; } = string.Empty;
  public string Route { get; set; } = string.Empty;
  public List<E2EArrange> Arrange { get; set; } = new List<E2EArrange>();
  public E2EAct? Act { get; set; }
  public List<E2EAssert> Assert { get; set; } = new List<E2EAssert>();
}

internal class E2ETestBase
{
  public string Name { get; set; } = string.Empty;
  public string Selector { get; set; } = string.Empty;
}

internal class E2EArrange : E2ETestBase
{
  public string Input { get; set; } = string.Empty;
}

internal class E2EAct : E2ETestBase
{
  public bool Click { get; set; } = false;
}

internal class E2EAssert : E2ETestBase
{
  public string Expected { get; set; } = string.Empty;
}