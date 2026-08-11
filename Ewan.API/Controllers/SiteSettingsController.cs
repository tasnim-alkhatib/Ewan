using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.SiteSettings;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteSettingsController : ControllerBase
    {
        private readonly ISiteSettingService _siteSettingService;

        public SiteSettingsController(ISiteSettingService siteSettingService)
        {
            _siteSettingService = siteSettingService;
        }

        /// <summary>كل الإعدادات كـ Dictionary - أسهل شكل للفرونت يستخدمه (settings["phone_number"])</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<Dictionary<string, SiteSettingDto>>>> GetPublic()
        {
            var result = await _siteSettingService.GetAllAsPublicDictionaryAsync();
            return Ok(ApiResponse<Dictionary<string, SiteSettingDto>>.Ok(result));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<SiteSettingDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _siteSettingService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<SiteSettingDto>>.Ok(result));
        }

        [HttpGet("{key}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SiteSettingDto>>> GetByKey(string key)
        {
            var setting = await _siteSettingService.GetByKeyAsync(key);
            if (setting is null)
                return NotFound(ApiResponse<SiteSettingDto>.Fail("الإعداد غير موجود"));

            return Ok(ApiResponse<SiteSettingDto>.Ok(setting));
        }

        /// <summary>إنشاء أو تحديث - لو الـ Key موجود بيتحدّث، لو مش موجود بيتعمل جديد</summary>
        [HttpPut]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<SiteSettingDto>>> Upsert([FromBody] UpsertSiteSettingRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var result = await _siteSettingService.UpsertAsync(request, adminUserId);
            return Ok(ApiResponse<SiteSettingDto>.Ok(result, "تم الحفظ بنجاح"));
        }

        [HttpDelete("{key}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(string key)
        {
            var deleted = await _siteSettingService.DeleteAsync(key);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("الإعداد غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
