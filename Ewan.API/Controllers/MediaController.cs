using Ewan.Application.DTOs.Common;
using Ewan.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ewan.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // رفع الصور من لوحة التحكم بس، مش عام
    public class MediaController : ControllerBase
    {
        private readonly IFileStorageService _storageService;

        // الحجم والصيغ المسموحة - القيم دي هي اللي المفروض زميلتك تعمل نفس التحقق بيها
        // في الفرونت قبل الإرسال (تجربة مستخدم أحسن، بدل ما تستنى رفض من السيرفر)
        private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2MB
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };

        public MediaController(IFileStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// رفع صورة واحدة. الفرونت بيبعتها كـ multipart/form-data بحقل اسمه "file"
        /// و query param اسمه "folder" (مثلا banners, offers, services)
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(MaxFileSizeBytes)]
        public async Task<ActionResult<ApiResponse<object>>> Upload(IFormFile file, [FromQuery] string folder = "general")
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("لم يتم اختيار ملف"));

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(ApiResponse<object>.Fail("حجم الصورة أكبر من 2 ميجابايت"));

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest(ApiResponse<object>.Fail("صيغة الملف غير مدعومة. المسموح: JPG, PNG, WEBP"));

            // تنظيف اسم الفولدر عشان محدش يبعت "../../" أو مسار غريب
            var safeFolder = string.IsNullOrWhiteSpace(folder) || folder.Any(c => !char.IsLetterOrDigit(c))
                ? "general"
                : folder.ToLowerInvariant();

            await using var stream = file.OpenReadStream();
            var url = await _storageService.UploadAsync(stream, file.FileName, file.ContentType, safeFolder);

            return Ok(ApiResponse<object>.Ok(new { imageUrl = url }, "تم رفع الصورة بنجاح"));
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return BadRequest(ApiResponse<object>.Fail("رابط الصورة مطلوب"));

            var deleted = await _storageService.DeleteAsync(imageUrl);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("الصورة غير موجودة"));

            return Ok(ApiResponse<object>.Ok(new { }, "تم حذف الصورة"));
        }
    }
}
