using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.ServiceItems;
using Ewan.Application.Interfaces;
using Ewan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceItemsController : ControllerBase
    {
        private readonly IServiceItemService _serviceItemService;

        public ServiceItemsController(IServiceItemService serviceItemService)
        {
            _serviceItemService = serviceItemService;
        }

        /// <summary>خدمات قطاع معين (أفراد/أعمال/صحي) - Endpoint عام</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ServiceItemDto>>>> GetPublic([FromQuery] Sector sector)
        {
            var result = await _serviceItemService.GetBySectorAsync(sector);
            return Ok(ApiResponse<List<ServiceItemDto>>.Ok(result));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceItemDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _serviceItemService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<ServiceItemDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ServiceItemDto>>> GetById(int id)
        {
            var item = await _serviceItemService.GetByIdAsync(id);
            if (item is null)
                return NotFound(ApiResponse<ServiceItemDto>.Fail("الخدمة غير موجودة"));

            return Ok(ApiResponse<ServiceItemDto>.Ok(item));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<ServiceItemDto>>> Create([FromBody] UpsertServiceItemRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var created = await _serviceItemService.CreateAsync(request, adminUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ServiceItemDto>.Ok(created, "تم إنشاء الخدمة بنجاح"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<ServiceItemDto>>> Update(int id, [FromBody] UpsertServiceItemRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _serviceItemService.UpdateAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<ServiceItemDto>.Fail("الخدمة غير موجودة"));

            return Ok(ApiResponse<ServiceItemDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _serviceItemService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("الخدمة غير موجودة"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        [HttpPost("reorder")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<object>>> Reorder([FromBody] Dictionary<int, int> idToSortOrder)
        {
            await _serviceItemService.ReorderAsync(idToSortOrder);
            return Ok(ApiResponse<object>.Ok(new { }, "تم إعادة الترتيب"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
