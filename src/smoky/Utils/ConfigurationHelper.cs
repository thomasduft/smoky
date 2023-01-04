using static tomware.Smoky.ConsoleHelper;

namespace tomware.Smoky;

internal static class ConfigurationHelper
{
  public static SmokyConfiguration GetConfiguration(string file)
  {
    SmokyConfiguration? configuration = null;
    try
    {
      WriteLine($"Reading config from file '{file}'");
      configuration = File.ReadAllText(file)
        .FromJson<SmokyConfiguration>();
    }
    catch (Exception ex)
    {
      Exit($"Error in reading config file! Exception: '{ex.Message}'");
    }

    return configuration!;
  }
}