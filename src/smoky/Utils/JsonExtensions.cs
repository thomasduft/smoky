using System.Text.Json;

namespace tomware.Smoky;

internal static class JsonExtensions
{
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
  };

  public static T FromJson<T>(this string json)
  {
    var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
    if (result is null) throw new InvalidDataException("Json string could not be deserialized");

    return result;
  }

  public static string ToJson<T>(this T obj) =>
      JsonSerializer.Serialize<T>(obj, _jsonOptions);
}