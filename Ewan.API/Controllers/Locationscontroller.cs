using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Locations;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<LocationDto>>>> GetPublic()
        {
            var result = await _locationService.GetActiveAsync();
            return Ok(ApiResponse<List<LocationDto>>.Ok(result));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<LocationDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _locationService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<LocationDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LocationDto>>> GetById(int id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location is null)
                return NotFound(ApiResponse<LocationDto>.Fail("الفرع غير موجود"));

            return Ok(ApiResponse<LocationDto>.Ok(location));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<LocationDto>>> Create([FromBody] UpsertLocationRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var created = await _locationService.CreateAsync(request, adminUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<LocationDto>.Ok(created, "تم إنشاء الفرع بنجاح"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<LocationDto>>> Update(int id, [FromBody] UpsertLocationRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _locationService.UpdateAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<LocationDto>.Fail("الفرع غير موجود"));

            return Ok(ApiResponse<LocationDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _locationService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("الفرع غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
