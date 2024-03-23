using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Plugins;

/// <summary>
/// A Sematic Kernel skill that interacts with ChatGPT
/// </summary>
internal class CoffeePlugin(KernelSettings kernelSettings)
{

    private readonly HttpClient client = new()
    {
        BaseAddress = new Uri(kernelSettings.DarkCafyUrl)
    };

    [KernelFunction("MakeCoffee")]
    [Description("Makes a coffee.")]
    async public Task<string> MakeCoffee()
    {
        // Print the state to the console
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Coffee is being made]");
        Console.ResetColor();

        string url = "coffee";
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            // Process the result as needed
            return "Le café est en cours de préparation";
        }
        else
        {
            // Handle the error case
            return "La machine à café n'est pas disponible pour le moment";
        }




    }
}
