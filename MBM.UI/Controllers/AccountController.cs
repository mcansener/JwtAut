using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using MBM.UI.Models;
using MBM.Common.Models.Api.Response;
using MBM.Common.Models.Api.Request;

namespace MBM.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _dalApiClient;
        private readonly HttpClient _tokenApiClient;
        private readonly string _jwtSecretKey;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _dalApiClient = httpClientFactory.CreateClient("DalApiClient");
            _tokenApiClient = httpClientFactory.CreateClient("TokenApiClient");
            _jwtSecretKey = configuration.GetValue<string>("JwtSettings:SecretKey")!;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var registerDto = new
            {
                model.Username,
                model.Email,
                model.Password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerDto), Encoding.UTF8, "application/json");

            var response = await _dalApiClient.PostAsync("api/user/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var registerResponse = JsonSerializer.Deserialize<RegisterResponse>(responseBody);

                if (registerResponse != null && !string.IsNullOrEmpty(registerResponse.UserId))
                {
                    var tokenResponse = await RequestJwtTokenFromJwtApiAsync(new TokenRequest
                    {
                        UserId = registerResponse.UserId,
                        Username = model.Username,
                        TokenSecretKey = _jwtSecretKey
                    });

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                    {
                        // Store the JWT token securely in an HttpOnly cookie
                        Response.Cookies.Append("jwtToken", tokenResponse.Token, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // Ensure the cookie is only sent over HTTPS
                                           // Set other cookie options as needed (e.g., expiration)
                        });

                        return RedirectToAction("Index", "Home"); // Redirect to dashboard or home page
                    }
                }
            }

            // Handle registration failure
            ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
            return View(model);
        }

        private async Task<TokenResponse?> RequestJwtTokenFromJwtApiAsync(TokenRequest model)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await _tokenApiClient.PostAsync("Token/generate", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                return tokenResponse;
            }

            return null;
        }
    }
}
