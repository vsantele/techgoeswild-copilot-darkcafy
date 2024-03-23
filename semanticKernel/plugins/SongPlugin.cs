using System.ComponentModel;
using Microsoft.SemanticKernel;
using Services;


namespace Plugins;

/// <summary>
/// A Sematic Kernel skill that interacts with ChatGPT
/// </summary>
internal class SongPlugin(SpotifyService spotify)
{


    [KernelFunction("PlayContent")]
    [Description("Joue un titre de musique")]
    async public Task<string> PlayContent([Description("Le titre d'une musique à jouer")] string title)
    {

        // Print the state to the console

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Searching a track for {title}]");
        Console.ResetColor();
        var track = await spotify.SearchTrack(title);
        if (track == null)
        {
            return "Could not find track";
        }
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Playing {track.Name} by {track.Artists[0].Name}");
        Console.ResetColor();

        await spotify.PlayTrack(track.Id);

        return "La musique " + track.Name + " par " + track.Artists[0].Name + " est en cours de lecture";
    }

    [KernelFunction("Pause")]
    [Description("Met en pause la musique")]
    async public Task<string> Pause()
    {
        // Print the state to the console
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Music is being paused]");
        Console.ResetColor();

        await spotify.Pause();

        return "La musique est en pause";
    }

    [KernelFunction("Resume")]
    [Description("Lance le lecteur de musique avec celle en cours")]
    async public Task<string> Resume()
    {
        // Print the state to the console
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Music is being resumed]");
        Console.ResetColor();

        var wasResume = await spotify.PlayTrack();

        if (!wasResume)
        {
            return "Une musique est déjà en cours de lecture";
        }

        return "La musique reprend";
    }


    [KernelFunction("SearchTracks")]
    [Description("Récupère une liste de titres de musique correspondant à une recherche")]
    async public Task<string> SearchTracks([Description("Le terme de recherche")] string query)
    {
        // Print the state to the console
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Searching tracks for {query}]");
        Console.ResetColor();
        var tracks = await spotify.SearchTracks(query);
        if (tracks == null)
        {
            return "Could not find tracks";
        }
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine($"[Found {tracks.Count()} tracks]");
        Console.ResetColor();

        return "Voici les titres trouvés : " + string.Join(", ", tracks.Select(t => t.Name + " Par " + t.Artists[0].Name));
    }


}