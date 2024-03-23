using System.Formats.Asn1;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace Services;

public class SpotifyService : IDisposable
{
    private SpotifyClient? spotify;

    private readonly EmbedIOAuthServer _server = new EmbedIOAuthServer(new Uri("http://localhost:5000"), 5000);

    private readonly string _deviceComputerId;
    private readonly string _clientId;

    public SpotifyService(KernelSettings kernelSettings)
    {
        _deviceComputerId = kernelSettings.SpotifyDeviceId;
        _clientId = kernelSettings.SpotifyClientId;
    }

    public async Task<bool> PlayTrack(string? trackId = null)
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }

        var cur = await GetCurrentlyPlaying();

        if (trackId is null && cur?.IsPlaying != true)
        {
            return false;
        }


        await spotify.Player.ResumePlayback(new PlayerResumePlaybackRequest
        {
            Uris = trackId is null ? null : [$"spotify:track:{trackId}"],
            DeviceId = _deviceComputerId is null ? null : _deviceComputerId
        });

        return true;
    }

    public async Task<IEnumerable<FullTrack>> SearchTracks(string query)
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }
        var response = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, query: query));
        return response?.Tracks.Items ?? Enumerable.Empty<FullTrack>();
    }

    public async Task<FullTrack?> SearchTrack(string query)
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }
        var response = await SearchTracks(query);
        return response.FirstOrDefault();
    }


    public async Task Pause()
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }

        await spotify.Player.PausePlayback();
    }

    public async Task AddToQueue(string trackId)
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }

        await spotify.Player.AddToQueue(new PlayerAddToQueueRequest($"spotify:track:{trackId}"));
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }

        var response = await spotify.Player.GetAvailableDevices();
        return response?.Devices ?? Enumerable.Empty<Device>();
    }

    public async Task<CurrentlyPlaying?> GetCurrentlyPlaying()
    {
        await CheckIsAuthorized();
        if (spotify == null)
        {
            throw new InvalidOperationException("Spotify is not authorized");
        }

        var response = await spotify.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest { });

        return response;
    }


    public async Task Authorize()
    {
        await _server.Start();

        _server.ImplictGrantReceived += OnImplicitGrantReceived;
        _server.ErrorReceived += OnErrorReceived;

        var request = new LoginRequest(_server.BaseUri, _clientId, LoginRequest.ResponseType.Token)
        {
            Scope = [Scopes.UserReadEmail, Scopes.AppRemoteControl, Scopes.UserReadPlaybackState, Scopes.UserModifyPlaybackState]
        };
        BrowserUtil.Open(request.ToUri());
        await Task.Delay(3000);
    }

    private async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
    {
        await _server.Stop();
        spotify = new SpotifyClient(response.AccessToken);
        // do calls with Spotify
    }

    private async Task OnErrorReceived(object sender, string error, string? state)
    {
        Console.WriteLine($"Aborting authorization, error received: {error}");
        await _server.Stop();
    }

    private async Task CheckIsAuthorized()
    {
        if (spotify == null)
        {
            await Authorize();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _server?.Dispose();
        }
    }
}
