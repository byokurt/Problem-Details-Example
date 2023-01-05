﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpContent content)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
                {
                    new JsonStringEnumConverter()
                }
        };

        T? value = await content.ReadFromJsonAsync<T>(jsonSerializerOptions);

        return value;
    }
}