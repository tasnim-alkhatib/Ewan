using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ewan.Application.DTOs.Banners;
using Ewan.Application.DTOs.Common;
using Ewan.Application.Interfaces;
using Ewan.Domain.Enums;

namespace Ewan.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        /// <summary>البانرات النشطة حسب مكانها في الموقع - Endpoint عام، بيستخدمه الفرونت العام</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<BannerDto>>>> GetPublic([FromQuery] BannerLocation location)
        {
            var result = await _bannerService.GetActiveByLocationAsync(location);
            return Ok(ApiResponse<List<BannerDto>>.Ok(result));
        }

        /// <summary>كل البانرات مع Pagination وبحث - لوحة التحكم فقط</summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<BannerDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _bannerService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<BannerDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<BannerDto>>> GetById(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner is null)
                return NotFound(ApiResponse<BannerDto>.Fail("البانر غير موجود"));

            return Ok(ApiResponse<BannerDto>.Ok(banner));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<BannerDto>>> Create([FromBody] UpsertBannerRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var created = await _bannerService.CreateAsync(request, adminUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<BannerDto>.Ok(created, "تم إنشاء البانر بنجاح"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<BannerDto>>> Update(int id, [FromBody] UpsertBannerRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _bannerService.UpdateAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<BannerDto>.Fail("البانر غير موجود"));

            return Ok(ApiResponse<BannerDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _bannerService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("البانر غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        /// <summary>لترتيب البانرات بالسحب والإفلات في اللوحة</summary>
        [HttpPost("reorder")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<object>>> Reorder([FromBody] Dictionary<int, int> idToSortOrder)
        {
            await _bannerService.ReorderAsync(idToSortOrder);
            return Ok(ApiResponse<object>.Ok(new { }, "تم إعادة الترتيب"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
