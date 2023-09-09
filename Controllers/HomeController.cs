using computerwala.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace computerwala.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;
        public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            var token = GetCachedToken();
            //token = await GetToken();

            //RestClient client = new RestClient("https://localhost:7089");
            //RestRequest restRequest = new RestRequest("Authentication/GetWeatherForecast", Method.Get);
            //restRequest.AddHeader("Content-Type", "application/json");

            //restRequest.AddHeader("Authorization", "Bearer " + token);

            //var resp = client.Execute(restRequest);
            //var content = resp.Content;

            return View();
        }

        public string GetCurrentUrl()
        {
            var currentUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            return currentUrl;
        }

        public async Task<string> GetCachedToken()
        {
            _logger.LogInformation($"In Cached token");

            if (_cache.TryGetValue("JwtToken", out string jwtToken))
            {
                // Token found in the cache
                _logger.LogInformation($"Returning cached token : {jwtToken}");
                return jwtToken;
            }
            else
            {
                // Token is not in the cache or has expired
                jwtToken =  await GetToken();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    // Cache the token with a specified expiration time (e.g., tokenExpirationMinutes)
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                    };
                    _logger.LogInformation($"Setting new token to cache: {jwtToken}");
                    _cache.Set("JwtToken", jwtToken, cacheEntryOptions);
                }
                //else
                //{
                //    // Handle the case where obtaining a new token fails
                //    //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                //    //return;
                //}
            }

            // Set the token in the request headers
            HttpContext.Request.Headers.Add("Authorization", "Bearer " + jwtToken);

            return jwtToken; // Or handle the absence of a token as needed

        }
        public async Task<string> GetToken()
        {
            var url = GetCurrentUrl();
            _logger.LogInformation($"Host : {url}");
            _logger.LogInformation($"Requesting token");
            RestClient client = new RestClient(url);
            RestRequest restRequest = new RestRequest("/api/Authentication/GetToken", Method.Get);
            restRequest.AddHeader("Content-Type", "application/json");

            var resp = await client.GetAsync(restRequest);
            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Api response : {resp.Content}");
                return JsonConvert.DeserializeObject<string>(resp.Content);
            }
            else
                return "";
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}