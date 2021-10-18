using MarciSpotifyApi.Api.Interfaces;
using MarciSpotifyApi.Api.Models;
using MarciSpotifyApi.Configuration;
using Microsoft.AspNetCore.Http.Extensions;
namespace MarciSpotifyApi.Authorization;

using MarciSpotifyApi.Utility;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

public class SpotifyLoginService : ISpotifyLoginService
{
    private readonly SpotifyCredentials spotifyCredentials;
    private readonly SpotifySettings spotifySettings;

    private static readonly ConcurrentDictionary<string, (string codeVerifier, string codeChallenge)> httpContextUniqueData
        = new();

    public SpotifyLoginService(IOptions<SpotifyCredentials> credentials, IOptions<SpotifySettings> settings)
    {
        spotifyCredentials = credentials.Value;
        spotifySettings = settings.Value;
    }

    public string GetRedirectUrl()
    {
        var queryBuilder = new QueryBuilder
        {
            { "response_type", "code" },
            { "client_id", spotifyCredentials.ClientId ?? string.Empty },
            { "scope", "user-read-private" },
            { "redirect_uri", spotifySettings.RedirectPath ?? string.Empty },
            { "code_challenge_method", "S256" }
        };

        var state = StringUtils.GenerateRandomString(20);
        var codeVerifier = StringUtils.GenerateRandomString(64);
        var codeChallenge = StringUtils.ComputeSHA256String(codeVerifier);

        httpContextUniqueData.GetOrAdd(state, (codeVerifier, codeChallenge));

        queryBuilder.Add("code_challenge", codeChallenge);
        queryBuilder.Add("state", state);

        return $"{spotifySettings.SpotifyAuthorizeUrl}{queryBuilder.ToQueryString()}";
    }

    public async Task<string> GetAccessTokenQueryBodyUrlEncodedAsync(string code, string state)
    {
        var accessToken = string.Empty;

        using (var client = new HttpClient())
        {
            if (string.IsNullOrEmpty(state))
            {
                return await Task.FromException<string>(new UnauthorizedAccessException());
            }

            if (!httpContextUniqueData.TryGetValue(state, out var challenge))
            {
                return await Task.FromException<string>(new UnauthorizedAccessException());
            }

            var codeVerifier = challenge.codeVerifier;
            var codeChallenge = challenge.codeChallenge;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", spotifyCredentials.ClientId ?? string.Empty),
                new KeyValuePair<string, string>("redirect_uri", spotifySettings.RedirectPath ?? string.Empty),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code_verifier", codeVerifier)
            });

            var resultContent = await client.PostAsync(spotifySettings.SpotifyTokenAccessUrl, content);

            var result = await resultContent.Content.ReadAsStringAsync();

            accessToken = System.Text.Json.JsonSerializer.Deserialize<SpotifyAccessTokenResponse>(result)?.AccessToken;
        }

        return accessToken ?? string.Empty;
    }
}
