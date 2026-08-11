using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.ServiceItems;
using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IServiceItemService
    {
        // بيستخدمه الموقع العام - يرجع خدمات قطاع معين بس (أفراد/أعمال/صحي)
        Task<List<ServiceItemDto>> GetBySectorAsync(Sector sector);

        Task<PagedResult<ServiceItemDto>> GetAllAsync(PagedRequest request);
        Task<ServiceItemDto?> GetByIdAsync(int id);
        Task<ServiceItemDto> CreateAsync(UpsertServiceItemRequest request, int adminUserId);
        Task<ServiceItemDto?> UpdateAsync(int id, UpsertServiceItemRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder);
    }
}
