using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Ewan.Application.DTOs.Banners;
using Ewan.Application.DTOs.Common;
using Ewan.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Ewan.Domain.Enums;

namespace Ewan.Infrastructure.Services
{
    public class BannerService : IBannerService
    {
        private readonly EwanDbContext _db;

        public BannerService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<List<BannerDto>> GetActiveByLocationAsync(BannerLocation location)
        {
            var now = DateTime.UtcNow;

            return await _db.Banners
                .Where(b => b.IsActive
                            && b.Location == location
                            && (b.StartDate == null || b.StartDate <= now)
                            && (b.EndDate == null || b.EndDate >= now))
                .OrderBy(b => b.SortOrder)
                .Select(b => ToDto(b))
                .ToListAsync();
        }

        public async Task<PagedResult<BannerDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.Banners.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(b => b.TitleAr.Contains(request.Search) || b.TitleEn.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(b => b.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => ToDto(b))
                .ToListAsync();

            return new PagedResult<BannerDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<BannerDto?> GetByIdAsync(int id)
        {
            var banner = await _db.Banners.FindAsync(id);
            return banner is null ? null : ToDto(banner);
        }

        public async Task<BannerDto> CreateAsync(UpsertBannerRequest request, int adminUserId)
        {
            var banner = new Banner
            {
                TitleAr = request.TitleAr,
                TitleEn = request.TitleEn,
                SubtitleAr = request.SubtitleAr,
                SubtitleEn = request.SubtitleEn,
                ImageUrl = request.ImageUrl,
                LinkUrl = request.LinkUrl,
                Location = request.Location,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedByUserId = adminUserId
            };

            _db.Banners.Add(banner);
            await _db.SaveChangesAsync();
            return ToDto(banner);
        }

        public async Task<BannerDto?> UpdateAsync(int id, UpsertBannerRequest request, int adminUserId)
        {
            var banner = await _db.Banners.FindAsync(id);
            if (banner is null) return null;

            banner.TitleAr = request.TitleAr;
            banner.TitleEn = request.TitleEn;
            banner.SubtitleAr = request.SubtitleAr;
            banner.SubtitleEn = request.SubtitleEn;
            banner.ImageUrl = request.ImageUrl;
            banner.LinkUrl = request.LinkUrl;
            banner.Location = request.Location;
            banner.SortOrder = request.SortOrder;
            banner.IsActive = request.IsActive;
            banner.StartDate = request.StartDate;
            banner.EndDate = request.EndDate;
            banner.UpdatedByUserId = adminUserId;

            await _db.SaveChangesAsync();
            return ToDto(banner);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var banner = await _db.Banners.FindAsync(id);
            if (banner is null) return false;

            banner.IsDeleted = true;   // Soft delete
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder)
        {
            var ids = idToSortOrder.Keys.ToList();
            var banners = await _db.Banners.Where(b => ids.Contains(b.Id)).ToListAsync();

            foreach (var banner in banners)
            {
                banner.SortOrder = idToSortOrder[banner.Id];
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private static BannerDto ToDto(Banner b) => new()
        {
            Id = b.Id,
            TitleAr = b.TitleAr,
            TitleEn = b.TitleEn,
            SubtitleAr = b.SubtitleAr,
            SubtitleEn = b.SubtitleEn,
            ImageUrl = b.ImageUrl,
            LinkUrl = b.LinkUrl,
            Location = b.Location,
            SortOrder = b.SortOrder,
            IsActive = b.IsActive
        };
    }
}
