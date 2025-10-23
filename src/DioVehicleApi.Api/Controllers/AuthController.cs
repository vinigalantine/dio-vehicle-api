using DioVehicleApi.Application.DTOs;
using DioVehicleApi.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DioVehicleApi.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required");
            }

            var token = await _authService.AuthenticateAsync(request.Username, request.Password);

            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new LoginResponse(token.Value, token.ExpiresAt));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Later will implement token blacklisting?
            return Ok(new { message = "Logged out" });
        }
    }
}
