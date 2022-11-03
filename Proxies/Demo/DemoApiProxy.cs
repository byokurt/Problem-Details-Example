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

    public async Task<DemoResponse> QueryTransaction(DemoRequest request)
    {
        string queryParams = request.ToQueryString();

        HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"v1/gifts?{queryParams}");

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            _logger.LogError("There was a problem while getting data from Demo Api. ResponseStatusCode:{ResponseStatusCode} ResponseBody:{ResponseBody}", (int) httpResponseMessage.StatusCode, responseContent);

            throw new ApplicationException($"There was a problem while getting data from Demo Api ResponseStatusCode:{(int) httpResponseMessage.StatusCode} ResponseBody:{responseContent}");
        }

        return await httpResponseMessage.Content.ReadAsJsonAsync<DemoResponse>();
    }
}