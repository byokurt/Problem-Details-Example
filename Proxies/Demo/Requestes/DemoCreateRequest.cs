using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Proxies.Demo.Requestes;

public class DemoCreateRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}