using DioVehicleApi.Application.Contracts.Auth;
using DioVehicleApi.Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DioVehicleApi.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Username and password are required");
                }

                var command = new LoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                };

                var response = await _mediator.Send(command, cancellationToken);

                if (response == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                    return Unauthorized("Invalid credentials");
                }

                _logger.LogInformation("User logged in successfully: {Username}", request.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", request.Username);
                return StatusCode(500, "An error occurred during login");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Later will implement token blacklisting?
            return Ok(new { message = "Logged out" });
        }
    }
}
