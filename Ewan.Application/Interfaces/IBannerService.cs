using Ewan.Application.DTOs.Banners;
using Ewan.Application.DTOs.Common;
using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IBannerService
    {
        // بيستخدمه الموقع العام (بدون تسجيل دخول) - يرجع البانرات النشطة بس حسب مكانها في الموقع
        Task<List<BannerDto>> GetActiveByLocationAsync(BannerLocation location);

        // بيستخدمه لوحة التحكم - يرجع كل البانرات مع Pagination وبحث
        Task<PagedResult<BannerDto>> GetAllAsync(PagedRequest request);

        Task<BannerDto?> GetByIdAsync(int id);
        Task<BannerDto> CreateAsync(UpsertBannerRequest request, int adminUserId);
        Task<BannerDto?> UpdateAsync(int id, UpsertBannerRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder); // للسحب والإفلات في اللوحة
    }
}
