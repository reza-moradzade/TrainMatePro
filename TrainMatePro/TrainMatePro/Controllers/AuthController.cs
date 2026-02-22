using Microsoft.AspNetCore.Mvc;
using TrainMatePro.DTOs;
using TrainMatePro.Services;

namespace TrainMatePro.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
                return Unauthorized(new { message = "ایمیل یا رمز عبور اشتباه است" });

            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = HttpContext.Items["User"] as UserDto;

            if (user == null)
                return Unauthorized(new { message = "لطفاً ابتدا وارد شوید" });

            return Ok(new { user });
        }
    }
}