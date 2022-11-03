using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Proxies.Demo.Requestes;

public class DemoRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}