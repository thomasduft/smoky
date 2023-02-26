using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace tomware.Smoky;

internal class E2ETest
{
  public string Name { get; set; } = string.Empty;
  public string Route { get; set; } = string.Empty;
  public List<E2ETestStep> Arrange { get; set; } = new List<E2ETestStep>();
  public E2ETestStep? Act { get; set; }
  public List<E2ETestStep> Assert { get; set; } = new List<E2ETestStep>();
}

internal class E2ETestStep
{
  public string Step { get; set; } = string.Empty;

  [JsonConverter(typeof(StringEnumConverter))]
  public LocatorType LocatorType { get; set; }

  public string Text { get; set; } = string.Empty;

  public string Value { get; set; } = string.Empty;

  [JsonConverter(typeof(StringEnumConverter))]
  public Microsoft.Playwright.AriaRole AriaRole { get; set; }

  [JsonConverter(typeof(StringEnumConverter))]
  public ActionType Action { get; set; }

  public string Expected { get; set; } = string.Empty;
}

internal enum LocatorType
{
  BySelector,
  GetByLabel,
  GetByRole,
  GetByTestId,
  GetByText
}

internal enum ActionType
{
  Click,
  Fill,
  IsVisible
}
