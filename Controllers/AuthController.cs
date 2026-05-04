using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }
     [HttpPost("register")]
       public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {   
        try
        {
            if (string.IsNullOrWhiteSpace(req.Username) ||
                string.IsNullOrWhiteSpace(req.Password) ||
                string.IsNullOrWhiteSpace(req.ConfirmPassword))
                return BadRequest(new { message = "Please fill in all fields" });

            var (success, message) = await _auth.RegisterAsync(
                req.Username, req.Password, req.ConfirmPassword);

            if (!success)
                return BadRequest(new { message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
        }
        return Ok(new { message = "Registration successful" });
    }

    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    // {
    //     if (string.IsNullOrWhiteSpace(req.Username) ||
    //         string.IsNullOrWhiteSpace(req.Password) ||
    //         string.IsNullOrWhiteSpace(req.ConfirmPassword))
    //         return BadRequest(new { message = "Please fill in all fields" });

    //     var (success, message) = await _auth.RegisterAsync(
    //         req.Username, req.Password, req.ConfirmPassword);

    //     if (!success)
    //         return BadRequest(new { message });

    //     return Ok(new { message });
    // }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] LoginRequest req)
    // {
    //     if (string.IsNullOrWhiteSpace(req.Username) ||
    //         string.IsNullOrWhiteSpace(req.Password))
    //         return BadRequest(new { message = "Please fill in all fields" });

    //     var (success, token, username) = await _auth.LoginAsync(req.Username, req.Password);

    //     if (!success)
    //         return Unauthorized(new { message = "Username or Password is incorrect" });

    //     return Ok(new { token, username });
    // }
}

public record RegisterRequest(string Username, string Password, string ConfirmPassword);
public record LoginRequest(string Username, string Password);
