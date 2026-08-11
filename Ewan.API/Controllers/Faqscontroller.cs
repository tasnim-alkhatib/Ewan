using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Faqs;
using Ewan.Application.Interfaces;
using Ewan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqsController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqsController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<FaqDto>>>> GetPublic([FromQuery] Sector sector)
        {
            var result = await _faqService.GetBySectorAsync(sector);
            return Ok(ApiResponse<List<FaqDto>>.Ok(result));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PagedResult<FaqDto>>>> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _faqService.GetAllAsync(request);
            return Ok(ApiResponse<PagedResult<FaqDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<FaqDto>>> GetById(int id)
        {
            var faq = await _faqService.GetByIdAsync(id);
            if (faq is null)
                return NotFound(ApiResponse<FaqDto>.Fail("السؤال غير موجود"));

            return Ok(ApiResponse<FaqDto>.Ok(faq));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<FaqDto>>> Create([FromBody] UpsertFaqRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var created = await _faqService.CreateAsync(request, adminUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<FaqDto>.Ok(created, "تم إنشاء السؤال بنجاح"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<FaqDto>>> Update(int id, [FromBody] UpsertFaqRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _faqService.UpdateAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<FaqDto>.Fail("السؤال غير موجود"));

            return Ok(ApiResponse<FaqDto>.Ok(updated, "تم التحديث بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _faqService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("السؤال غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        [HttpPost("reorder")]
        [Authorize(Roles = "SuperAdmin,Editor,ContentManager")]
        public async Task<ActionResult<ApiResponse<object>>> Reorder([FromBody] Dictionary<int, int> idToSortOrder)
        {
            await _faqService.ReorderAsync(idToSortOrder);
            return Ok(ApiResponse<object>.Ok(new { }, "تم إعادة الترتيب"));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
