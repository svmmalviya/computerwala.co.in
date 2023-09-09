using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace tokentest1.MW
{


    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using RestSharp;

    public class JwtTokenCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public JwtTokenCacheMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_cache.TryGetValue("JwtToken", out string jwtToken) || string.IsNullOrEmpty(jwtToken))
            {
                // Token is not in cache or is empty; obtain a new token
                jwtToken = await GetToken(); // Implement this method

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    // Cache the token with a specified expiration time (e.g., tokenExpirationMinutes)
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    };
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
            context.Request.Headers["Authorization"] = "Bearer " + jwtToken;

            await _next(context);
        }

        // Implement the ObtainNewToken method as needed
        public async Task<string> GetToken()
        {
            RestClient client = new RestClient("https://localhost:7037");
            RestRequest restRequest = new RestRequest("api/Authentication/GetToken", Method.Get);
            restRequest.AddHeader("Content-Type", "application/json");

            var resp = await client.GetAsync(restRequest);
            if (resp.IsSuccessStatusCode)
                return resp.Content;
            else
                return "";
        }
    }


}
