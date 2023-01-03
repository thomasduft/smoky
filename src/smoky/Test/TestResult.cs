namespace tomware.Smoky;

internal record TestResult
{
  private string Actual { get; set; }

  private string Reason { get; set; }

  public string Name { get; private set; }

  public TestStatus Status { get; private set; }

  public string FailCause
  {
    get
    {
      return !string.IsNullOrWhiteSpace(Actual)
        ? Actual
        : Reason;
    }
  }

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

  public static TestResult FailedWithOtherReason(string name, string reason)
  {
    return new TestResult
    {
      Name = name,
      Status = TestStatus.Failed,
      Reason = reason
    };
  }
}
