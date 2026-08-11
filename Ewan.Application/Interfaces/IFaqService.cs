using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Faqs;
using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IFaqService
    {
        Task<List<FaqDto>> GetBySectorAsync(Sector sector);
        Task<PagedResult<FaqDto>> GetAllAsync(PagedRequest request);
        Task<FaqDto?> GetByIdAsync(int id);
        Task<FaqDto> CreateAsync(UpsertFaqRequest request, int adminUserId);
        Task<FaqDto?> UpdateAsync(int id, UpsertFaqRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder);
    }
}
