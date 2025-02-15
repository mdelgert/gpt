
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Load configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Load Azure OpenAI settings
        var azureSettings = config.GetSection("AzureOpenAI");
        string apiKey = azureSettings["ApiKey"];
        string endpoint = azureSettings["Endpoint"];
        string model = azureSettings["Model"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(model))
        {
            Console.WriteLine("Azure OpenAI configuration is incomplete. Please update appsettings.json.");
            return;
        }

        var chatService = new ChatService(apiKey, endpoint, model);

        Console.WriteLine("Welcome to Azure OpenAI Chat! (Type 'exit' to quit)");

        while (true)
        {
            Console.Write("You: ");
            string userInput = Console.ReadLine();

            if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase))
                break;

            string response = await chatService.GetResponseAsync(userInput);
            Console.WriteLine($"AI: {response}");
        }
    }
}