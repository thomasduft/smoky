namespace tomware.Smoky;

internal record TestResult
{
  public string Name { get; private set; }

  public TestStatus Status { get; private set; }

  public string Actual { get; set; }

  public static TestResult Passed(string name)
  {
    return new TestResult
    {
      Name = name,
      Status = TestStatus.Passed
    };
  }

  public static TestResult Failed(string name, string actual)
  {
    return new TestResult
    {
      Name = name,
      Status = TestStatus.Failed,
      Actual = actual
    };
  }
}
