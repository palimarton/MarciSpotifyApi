using MarciSpotifyApi.Api.Interfaces;
using MarciSpotifyApi.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MarciSpotifyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SpotifyController
    {
        private readonly ISpotifyLoginService spotifyLoginService;

        public SpotifyController(ISpotifyLoginService spotifyLoginService)
        {
            this.spotifyLoginService = spotifyLoginService;
        }

        [HttpGet]
        public ActionResult HealthCheck()
        {
            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult Login()
        {
            // Redirect
            var redirectUrl = spotifyLoginService.GetRedirectUrl();

            return new RedirectResult(redirectUrl);
        }

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
}
