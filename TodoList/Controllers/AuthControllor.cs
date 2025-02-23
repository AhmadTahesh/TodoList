using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Interfaces;
using TodoList.Requests;


namespace TodoList.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required");

            var session = await _authService.SignUp(request);
            return Ok(session);
        }
        catch (Exception ex) when (ex.Message.Contains("User already registered"))
        {
            return Conflict("User already exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignUp failed");
            return StatusCode(500, "An error occurred during sign up");
        }
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        try
        {
            var session = await _authService.SignIn(request);
            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignIn failed");
            return Unauthorized("Invalid credentials");
        }
    }

    [HttpPost("signout")]
    [Authorize]
    public async Task<IActionResult> SignOut()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            await _authService.SignOut(token);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignOut failed");
            return StatusCode(500, "Failed to sign out");
        }
    }
}
