using Ewan.Application.DTOs.Auth;
using Ewan.Application.DTOs.Common;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
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
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (result is null)
                return Unauthorized(ApiResponse<LoginResponse>.Fail("الإيميل أو كلمة المرور غير صحيحة"));

            return Ok(ApiResponse<LoginResponse>.Ok(result, "تم تسجيل الدخول بنجاح"));
        }
    }
}
