using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using AspNet.Security.OAuth.Discord;

namespace GroundhogWeb.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(IHttpClientFactory clientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {

            var properties = new AuthenticationProperties { RedirectUri = Url.Action("Callback") };
            return new ChallengeResult("Discord", properties);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return BadRequest(); // or you might want to redirect to an error page
            }

            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
            var tokenType = authenticateResult.Properties.GetTokenValue("token_type");
            var expiresIn = authenticateResult.Properties.GetTokenValue("expires_in");
            var refreshToken = authenticateResult.Properties.GetTokenValue("refresh_token");
            var scope = authenticateResult.Properties.GetTokenValue("scope");

            // Now you can use these tokens
            // 列出使用者的 Discord 資料
            var request = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
            request.Headers.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            // 返回 401 代表 Token 過期，需要重新登入
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Redirect("/auth/login");
            }

            // 其他錯誤，直接返回 500
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // 如果成功返回使用者資料
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
            if (user != null)
            {
                return Ok(user);
            }



            // Redirect to home page or wherever you want
            return Redirect("/");
        }



    }
}
