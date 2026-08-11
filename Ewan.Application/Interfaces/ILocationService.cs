using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface ILocationService
    {
        // بيستخدمه الموقع العام - كل الفروع النشطة (مفيش تقسيم بقطاع هنا)
        Task<List<LocationDto>> GetActiveAsync();

        Task<PagedResult<LocationDto>> GetAllAsync(PagedRequest request);
        Task<LocationDto?> GetByIdAsync(int id);
        Task<LocationDto> CreateAsync(UpsertLocationRequest request, int adminUserId);
        Task<LocationDto?> UpdateAsync(int id, UpsertLocationRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);
    }
}
