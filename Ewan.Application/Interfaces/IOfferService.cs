using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.offers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IOfferService
    {
        // بيستخدمه الموقع العام - يرجع العروض النشطة بس (اللي جوه فترة الصلاحية بتاعتها)
        Task<List<OfferDto>> GetActiveAsync();

        // بيستخدمه لوحة التحكم - كل العروض مع Pagination وبحث
        Task<PagedResult<OfferDto>> GetAllAsync(PagedRequest request);

        Task<OfferDto?> GetByIdAsync(int id);
        Task<OfferDto> CreateAsync(UpsertOfferRequest request, int adminUserId);
        Task<OfferDto?> UpdateAsync(int id, UpsertOfferRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder);
    }
}
