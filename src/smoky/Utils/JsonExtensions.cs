using Newtonsoft.Json;

namespace tomware.Smoky;

internal static class JsonExtensions
{
  public static T FromJson<T>(this string json)
  {
    var result = JsonConvert.DeserializeObject<T>(json);
    if (result is null) throw new InvalidDataException("Json string could not be deserialized");

    return result;
  }

  internal static string ToJsonForInitCommand<T>(this T obj)
  {
    return JsonConvert.SerializeObject(obj, Formatting.Indented);
  }
}