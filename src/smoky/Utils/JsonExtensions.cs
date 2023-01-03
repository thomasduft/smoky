using System.Text.Json;

namespace tomware.Smoky;

public static class JsonExtensions
{
  private static readonly JsonSerializerOptions _jsonOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
  };

  public static T FromJson<T>(this string json) =>
      JsonSerializer.Deserialize<T>(json, _jsonOptions);

  public static string ToJson<T>(this T obj) =>
      JsonSerializer.Serialize<T>(obj, _jsonOptions);
}