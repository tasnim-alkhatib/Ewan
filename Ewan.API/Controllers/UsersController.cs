using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Users;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // إدارة المستخدمين لـ SuperAdmin بس، من غير استثناء
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AdminUserDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _userService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<AdminUserDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user is null)
                return NotFound(ApiResponse<AdminUserDto>.Fail("المستخدم غير موجود"));

            return Ok(ApiResponse<AdminUserDto>.Ok(user));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> Create([FromBody] CreateAdminUserRequest request)
        {
            var created = await _userService.CreateAsync(request);
            if (created is null)
                return Conflict(ApiResponse<AdminUserDto>.Fail("الإيميل ده مستخدم بالفعل"));

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<AdminUserDto>.Ok(created, "تم إنشاء المستخدم بنجاح"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> Update(int id, [FromBody] UpdateAdminUserRequest request)
        {
            var updated = await _userService.UpdateAsync(id, request);
            if (updated is null)
                return NotFound(ApiResponse<AdminUserDto>.Fail("المستخدم غير موجود"));

            return Ok(ApiResponse<AdminUserDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            var deleted = await _userService.DeleteAsync(id, currentUserId);
            if (!deleted)
                return BadRequest(ApiResponse<object>.Fail("تعذّر حذف المستخدم (إما غير موجود أو إنه حسابك الحالي)"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        /// <summary>أي مستخدم مسجل دخول يقدر يغيّر باسورده هو بس (مش شرط SuperAdmin)</summary>
        [HttpPost("change-password")]
        [Authorize] // يلغي قيد SuperAdmin بتاع الـ Controller، أي مستخدم مسجل دخول يقدر يستخدمه
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var success = await _userService.ChangePasswordAsync(currentUserId, request);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("كلمة المرور الحالية غير صحيحة"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم تغيير كلمة المرور بنجاح"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
