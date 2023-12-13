using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.Extensions;
using ComputerWala.Interfaces;
using System.Runtime.Caching;
using MemoryCache = System.Runtime.Caching.MemoryCache;
using Newtonsoft.Json.Linq;

namespace computerwala.MWs
{

    public class JwtTokenCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IAuthentication _authentication;

        public JwtTokenCacheMiddleware(RequestDelegate next, IMemoryCache cache, IAuthentication authentication)
        {
            _next = next;
            _cache = cache;
            _authentication = authentication;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (!_cache.TryGetValue("JwtToken", out string jwtToken) || string.IsNullOrEmpty(jwtToken))
            {
                // Token is not in cache or is empty; obtain a new token

                if (context.Session.GetInt32("jwtRequest") == null)
                {
                    jwtToken = _authentication.GetToken(); // Implement this method
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedJwtToken = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    var validMinute = (decodedJwtToken.ValidFrom.Minute - decodedJwtToken.ValidTo.Minute);
                    // Cache the token with a specified expiration time (e.g., tokenExpirationMinutes)
                    //var cacheEntryOptions = new MemoryCacheEntryOptions
                    //{
                    //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(validMinute)
                    //};

                    //_cache.Set("JwtToken", jwtToken, cacheEntryOptions);

                    MemoryCache cache = new MemoryCache("MyCache");
                    CacheItemPolicy policy = new CacheItemPolicy();

                    // Set an absolute expiration time 18 minutes from now
                    policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(60);

                    // Add your item to the cache with this policy

                    //cache.Set("JwtToken", jwtToken, policy);
                }

            }

            // Set the token in the request headers

            context.Response.Headers.Add("Authorization", "Bearer " + jwtToken);

            await _next(context);
        }

        // Implement the ObtainNewToken method as needed

    }


}
