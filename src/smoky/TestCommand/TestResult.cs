namespace tomware.Smoky;

internal record TestResult
{
  private string Actual { get; set; } = string.Empty;

  private string Reason { get; set; } = string.Empty;

  public string Name { get; private set; } = string.Empty;

  public string TestStep { get; set; } = string.Empty;

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

  public static TestResult Failed(string testName, string testStep, string actual)
  {
    return new TestResult
    {
      Name = testName,
      TestStep = testStep,
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