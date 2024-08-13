using System.Reflection;

namespace tomware.Smoky;

public static class Resources
{
  public static string SmokyJsonSchema = "smoky-schema.json";
}

public static class ResourceLoader
{
  public static string GetResource(string template)
  {
    var assembly = Assembly.GetExecutingAssembly();
    var resourcePath = assembly.ManifestModule.Name.Replace(".dll", string.Empty);
    var resourceName = $"{resourcePath}.Resources.{template}";

    using (var stream = assembly.GetManifestResourceStream(resourceName))
    using (StreamReader reader = new(stream!))
    {
      return reader.ReadToEnd();
    }

    throw new FileNotFoundException($"Resource with name '{template}' does not exist!");
  }
}