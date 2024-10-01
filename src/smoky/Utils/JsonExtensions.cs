using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace tomware.Smoky;

internal static class JsonExtensions
{
  public static T FromJson<T>(this string json)
  {
    return JsonConvert.DeserializeObject<T>(json)
      ?? throw new InvalidDataException("Json string could not be deserialized");
  }

  internal static string ToJsonForInitCommand<T>(this T obj)
  {
    return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
    {
      Formatting = Formatting.Indented,
      Converters = [new StringEnumConverter()]
    });
  }
}