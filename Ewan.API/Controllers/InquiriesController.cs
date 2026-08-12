using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Inquiries;
using Ewan.Application.Interfaces;
using Ewan.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InquiriesController : ControllerBase
    {
        private readonly IInquiryService _inquiryService;

        public InquiriesController(IInquiryService inquiryService)
        {
            _inquiryService = inquiryService;
        }

        /// <summary>استقبال فورم استفسار من الموقع العام - بدون تسجيل دخول</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<InquiryDto>>> Create([FromBody] CreateInquiryRequest request)
        {
            var created = await _inquiryService.CreateAsync(request);
            return Ok(ApiResponse<InquiryDto>.Ok(created, "تم إرسال طلبك بنجاح، هيتواصل معاك فريقنا قريبًا"));
        }

        /// <summary>كل الاستفسارات مع فلترة بالحالة والقطاع - لوحة التحكم فقط</summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Editor,LeadsViewer")]
        public async Task<ActionResult<ApiResponse<PagedResult<InquiryDto>>>> GetAll(
            [FromQuery] PagedRequest request,
            [FromQuery] InquiryStatus? status,
            [FromQuery] Sector? sector)
        {
            var result = await _inquiryService.GetAllAsync(request, status, sector);
            return Ok(ApiResponse<PagedResult<InquiryDto>>.Ok(result));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor,LeadsViewer")]
        public async Task<ActionResult<ApiResponse<InquiryDto>>> GetById(int id)
        {
            var inquiry = await _inquiryService.GetByIdAsync(id);
            if (inquiry is null)
                return NotFound(ApiResponse<InquiryDto>.Fail("الاستفسار غير موجود"));

            return Ok(ApiResponse<InquiryDto>.Ok(inquiry));
        }

        /// <summary>تحديث حالة الاستفسار (متابعة، مغلق...) وإضافة ملاحظات فريق المبيعات</summary>
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "SuperAdmin,Editor,LeadsViewer")]
        public async Task<ActionResult<ApiResponse<InquiryDto>>> UpdateStatus(int id, [FromBody] UpdateInquiryStatusRequest request)
        {
            var adminUserId = GetCurrentUserId();
            var updated = await _inquiryService.UpdateStatusAsync(id, request, adminUserId);
            if (updated is null)
                return NotFound(ApiResponse<InquiryDto>.Fail("الاستفسار غير موجود"));

            return Ok(ApiResponse<InquiryDto>.Ok(updated, "تم تحديث الحالة بنجاح"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin,Editor")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var deleted = await _inquiryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("الاستفسار غير موجود"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم الحذف بنجاح"));
        }

        /// <summary>عدد الاستفسارات الجديدة - لعرض Badge في اللوحة (الفرونت بيستدعيه كل 30-60 ثانية)</summary>
        [HttpGet("new-count")]
        [Authorize(Roles = "SuperAdmin,Editor,LeadsViewer")]
        public async Task<ActionResult<ApiResponse<int>>> GetNewCount()
        {
            var count = await _inquiryService.GetNewCountAsync();
            return Ok(ApiResponse<int>.Ok(count));
        }

        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst("uid")?.Value;
            return int.TryParse(idClaim, out var id) ? id : 0;
        }
    }
}
