using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.ServiceItems;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Domain.Enums;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class ServiceItemService : IServiceItemService
    {
        private readonly EwanDbContext _db;

        public ServiceItemService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<List<ServiceItemDto>> GetBySectorAsync(Sector sector)
        {
            return await _db.ServiceItems
                .Where(s => s.IsActive && s.Sector == sector)
                .OrderBy(s => s.SortOrder)
                .Select(s => ToDto(s))
                .ToListAsync();
        }

        public async Task<PagedResult<ServiceItemDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.ServiceItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(s => s.NameAr.Contains(request.Search) || s.NameEn.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => ToDto(s))
                .ToListAsync();

            return new PagedResult<ServiceItemDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ServiceItemDto?> GetByIdAsync(int id)
        {
            var item = await _db.ServiceItems.FindAsync(id);
            return item is null ? null : ToDto(item);
        }

        public async Task<ServiceItemDto> CreateAsync(UpsertServiceItemRequest request, int adminUserId)
        {
            var item = new ServiceItem
            {
                Sector = request.Sector,
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                DescriptionAr = request.DescriptionAr,
                DescriptionEn = request.DescriptionEn,
                ImageUrl = request.ImageUrl,
                Slug = request.Slug,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedByUserId = adminUserId
            };

            _db.ServiceItems.Add(item);
            await _db.SaveChangesAsync();
            return ToDto(item);
        }

        public async Task<ServiceItemDto?> UpdateAsync(int id, UpsertServiceItemRequest request, int adminUserId)
        {
            var item = await _db.ServiceItems.FindAsync(id);
            if (item is null) return null;

            item.Sector = request.Sector;
            item.NameAr = request.NameAr;
            item.NameEn = request.NameEn;
            item.DescriptionAr = request.DescriptionAr;
            item.DescriptionEn = request.DescriptionEn;
            item.ImageUrl = request.ImageUrl;
            item.Slug = request.Slug;
            item.SortOrder = request.SortOrder;
            item.IsActive = request.IsActive;
            item.UpdatedByUserId = adminUserId;

            await _db.SaveChangesAsync();
            return ToDto(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _db.ServiceItems.FindAsync(id);
            if (item is null) return false;

            item.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder)
        {
            var ids = idToSortOrder.Keys.ToList();
            var items = await _db.ServiceItems.Where(s => ids.Contains(s.Id)).ToListAsync();

            foreach (var item in items)
            {
                item.SortOrder = idToSortOrder[item.Id];
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private static ServiceItemDto ToDto(ServiceItem s) => new()
        {
            Id = s.Id,
            Sector = s.Sector,
            NameAr = s.NameAr,
            NameEn = s.NameEn,
            DescriptionAr = s.DescriptionAr,
            DescriptionEn = s.DescriptionEn,
            ImageUrl = s.ImageUrl,
            Slug = s.Slug,
            SortOrder = s.SortOrder,
            IsActive = s.IsActive
        };
    }
}
