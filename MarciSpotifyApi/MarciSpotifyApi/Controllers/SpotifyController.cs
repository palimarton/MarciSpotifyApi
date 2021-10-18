using MarciSpotifyApi.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarciSpotifyApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class SpotifyController
{
    private readonly ISpotifyLoginService spotifyLoginService;

    public SpotifyController(ISpotifyLoginService spotifyLoginService) 
        => this.spotifyLoginService = spotifyLoginService;

    [HttpGet]
    public ActionResult HealthCheck() 
        => new EmptyResult();

    [HttpGet]
    public ActionResult Login()
        => new RedirectResult(spotifyLoginService.GetRedirectUrl());

    [HttpGet]
    public async Task<ActionResult> LoginCallback(string code, string state)
    {
        var result = $"code: {code}\nstate: {state}";

        if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(state))
        {
            var authToken = await spotifyLoginService.GetAccessTokenQueryBodyUrlEncodedAsync(code, state);
            result = $"code: {code}\nstate: {state}\ntoken: {authToken}";
        }

        return new ContentResult()
        {
            Content = result
        };
    }
}