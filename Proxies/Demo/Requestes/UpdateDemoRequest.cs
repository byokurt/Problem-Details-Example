using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Proxies.Demo.Requestes;

public class UpdateDemoRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}