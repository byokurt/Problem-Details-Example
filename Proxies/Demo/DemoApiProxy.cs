using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProblemDetailsExample.Extensions;
using ProblemDetailsExample.Proxies.Demo.Requestes;
using ProblemDetailsExample.Proxies.Demo.Responses;

namespace ProblemDetailsExample.Proxies.Demo;

public class DemoApiProxy : IDemoApiProxy
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DemoApiProxy> _logger;

    public DemoApiProxy(HttpClient httpClient, ILogger<DemoApiProxy> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DemoResponse?> QueryTransaction(DemoRequest request)
    {
        string queryParams = request.ToQueryString();

        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"v1/gifts?{queryParams}");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogError("There was a problem while getting data from Demo Api. ResponseStatusCode:{ResponseStatusCode} ResponseBody:{ResponseBody}", (int)httpResponseMessage.StatusCode, responseContent);

            throw new ApplicationException($"There was a problem while getting data from Demo Api ResponseStatusCode:{(int)httpResponseMessage.StatusCode} ResponseBody:{responseContent}");
        }

        DemoResponse? demoResponse = await httpResponseMessage.Content.ReadAsJsonAsync<DemoResponse>();

        return demoResponse;
    }

    public async Task UpdateUser(int id, UpdateDemoRequest request)
    {
        JsonPatchDocument document = new JsonPatchDocument();

        string requestModel = JsonSerializer.Serialize(document.Operations);

        StringContent httpContent = new StringContent(requestModel, Encoding.UTF8, "application/json-patch+json");

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"v1/demo/{id}")
        {
            Content = httpContent
        };

        HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

        if (!responseMessage.IsSuccessStatusCode)
        {
            ProblemDetails? problemDetails = await responseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

            if (problemDetails != null)
            {
                _logger.LogError($"Patch api call failed. Status Code: {problemDetails.Status}");
            }
        }
    }
}