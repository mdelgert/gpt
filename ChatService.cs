
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ChatService
{
    private readonly string _apiKey;
    private readonly string _endpoint;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    public ChatService(string apiKey, string endpoint, string model)
    {
        _apiKey = apiKey;
        _endpoint = endpoint;
        _model = model;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetResponseAsync(string userInput)
    {
        var requestUrl = $"{_endpoint}/openai/deployments/{_model}/chat/completions?api-version=2023-03-15-preview";

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = userInput }
            }
        };

        var requestJson = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestUrl),
            Headers =
            {
                { "api-key", _apiKey }
            },
            Content = content
        };

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return "Error occurred. Please try again.";
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<JsonElement>(responseJson);

            var aiMessage = responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
            return aiMessage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return "An error occurred. Please try again.";
        }
    }
}