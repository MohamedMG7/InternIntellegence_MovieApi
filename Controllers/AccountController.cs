using InternIntellegence_MovieApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternIntellegence_MovieApi.Controllers{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase{
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _accountService.RegisterAsync(request.Email, request.Password);
                if (result.Succeeded)
                {
                    return Ok(new { message = "User registered successfully" });
                }
                return BadRequest(new { message = "User registration failed", errors = result.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while registering the user", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (result, token) = await _accountService.LoginAsync(request.Email, request.Password);
                if (result.Succeeded)
                {
                    return Ok(new { token });
                }
                return Unauthorized(new { message = "Invalid login attempt" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while logging in", details = ex.Message });
            }
        }
    }

        public class RegisterRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }
}

