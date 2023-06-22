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
using GroundhogWeb.Repositories;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace GroundhogWeb.Controllers
{
    [Route("/auth")]
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly UsersRepository _usersRepo;
        public AuthController(IHttpClientFactory clientFactory, IConfiguration configuration, UsersRepository usersRepository)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
            _usersRepo = usersRepository;
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
            var user = CreateUserFromDictionary(JsonSerializer.Deserialize<Dictionary<string, object>>(content));
            if (user != null)
            {
                // 檢查Mongo是否存在該使用者
                var checkIfUserExists = await _usersRepo.GetByIdAsync(user.Id);
                if (checkIfUserExists is null)
                {
                    // 如果不存在，則新增使用者
                    await _usersRepo.CreateAsync(user);
                }
                else
                {
                    // 如果存在，則更新使用者
                    await _usersRepo.UpdateAsync(user.Id, user);
                }

                // 使用JWT加密使用者id，並存入Cookie
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(Convert.ToDouble(7));

                var token = new JwtSecurityToken(
                    issuer: _configuration["ClientDomain"],
                    audience: _configuration["ClientDomain"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                Response.Cookies.Append(
                    "JwtToken",
                    jwtToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = expires
                    }
                );

                // 登入成功，返回Client端Domain
                return Redirect(_configuration["ClientDomain"]);
            }



            // Redirect to home page or wherever you want
            return Redirect(_configuration["ClientDomain"]);

        }

        private Models.User CreateUserFromDictionary(Dictionary<string, object> user)
        {
            return new Models.User
            {
                Id = user["id"].ToString(),
                Username = user["username"].ToString(),
                Discriminator = user["discriminator"].ToString(),
                Avatar = user["avatar"]?.ToString(),
                Email = user["email"]?.ToString(),
                Verified = user["verified"] is bool verified && verified,
                Locale = user["locale"]?.ToString(),
                MfaEnabled = user["mfa_enabled"] is bool mfaEnabled && mfaEnabled,
                Flags = user["flags"] is int flags ? flags : 0,
                PremiumType = user["premium_type"] is int premiumType ? premiumType : 0,
                PublicFlags = user["public_flags"] is int publicFlags ? publicFlags : 0
            };
        }



    }
}
