using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerOptions jsonSerializerOptions = null)
    {
        if (jsonSerializerOptions == null)
        {
            jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            };
        }

        string json = await content.ReadAsStringAsync();

        T value = JsonSerializer.Deserialize<T>(json, jsonSerializerOptions);

        return value;
    }
}