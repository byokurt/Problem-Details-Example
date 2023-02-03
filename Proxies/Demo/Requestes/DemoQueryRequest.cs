using System.Text.Json.Serialization;

namespace ProblemDetailsExample.Proxies.Demo.Requestes;

public class DemoQueryRequest
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}