using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.offers;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        /// <summary>العروض النشطة حاليًا - Endpoint عام لصفحة العروض في الموقع</summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<OfferDto>>>> GetPublic()
        {
            var result = await _offerService.GetActiveAsync();
            return Ok(ApiResponse<List<OfferDto>>.Ok(result));
        }

        /// <summary>كل العروض مع Pagination وبحث - لوحة التحكم فقط</summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<OfferDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _offerService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<OfferDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OfferDto>>> GetById(int id)
        {
            var offer = await _offerService.GetByIdAsync(id);
            if (offer is null)
                return NotFound(ApiResponse<OfferDto>.Fail("العرض غير موجود"));

            return Ok(ApiResponse<OfferDto>.Ok(offer));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<OfferDto>>> Create([FromBody] UpsertOfferRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var created = await _offerService.CreateAsync(request, adminUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<OfferDto>.Ok(created, "تم إنشاء العرض بنجاح"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<OfferDto>>> Update(int id, [FromBody] UpsertOfferRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _offerService.UpdateAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<OfferDto>.Fail("العرض غير موجود"));

            return Ok(ApiResponse<OfferDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _offerService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("العرض غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        [HttpPost("reorder")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<object>>> Reorder([FromBody] Dictionary<int, int> idToSortOrder)
        {
            await _offerService.ReorderAsync(idToSortOrder);
            return Ok(ApiResponse<object>.Ok(new { }, "تم إعادة الترتيب"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
