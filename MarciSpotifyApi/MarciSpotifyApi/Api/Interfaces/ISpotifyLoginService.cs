namespace MarciSpotifyApi.Api.Interfaces;

public interface ISpotifyLoginService
{
    Task<string> GetAccessTokenQueryBodyUrlEncodedAsync(string code, string state);
    string GetRedirectUrl();
}
