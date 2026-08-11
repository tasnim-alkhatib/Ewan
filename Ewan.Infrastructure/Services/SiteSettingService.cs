using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.SiteSettings;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Ewan.Infrastructure.Services
{
    public class SiteSettingService : ISiteSettingService
    {
        private readonly EwanDbContext _db;

        public SiteSettingService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<Dictionary<string, SiteSettingDto>> GetAllAsPublicDictionaryAsync()
        {
            var settings = await _db.SiteSettings
                .Where(s => s.IsActive)
                .Select(s => ToDto(s))
                .ToListAsync();

            return settings.ToDictionary(s => s.Key, s => s);
        }

        public async Task<PagedResult<SiteSettingDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.SiteSettings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(s => s.Key.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Key)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => ToDto(s))
                .ToListAsync();

            return new PagedResult<SiteSettingDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<SiteSettingDto?> GetByKeyAsync(string key)
        {
            var setting = await _db.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
            return setting is null ? null : ToDto(setting);
        }

        public async Task<SiteSettingDto> UpsertAsync(UpsertSiteSettingRequest request, int adminUserId)
        {
            var existing = await _db.SiteSettings.FirstOrDefaultAsync(s => s.Key == request.Key);

            if (existing is not null)
            {
                existing.ValueAr = request.ValueAr;
                existing.ValueEn = request.ValueEn;
                existing.UpdatedByUserId = adminUserId;
                await _db.SaveChangesAsync();
                return ToDto(existing);
            }

            var setting = new SiteSetting
            {
                Key = request.Key,
                ValueAr = request.ValueAr,
                ValueEn = request.ValueEn,
                CreatedByUserId = adminUserId
            };

            _db.SiteSettings.Add(setting);
            await _db.SaveChangesAsync();
            return ToDto(setting);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var setting = await _db.SiteSettings.FirstOrDefaultAsync(s => s.Key == key);
            if (setting is null) return false;

            setting.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        private static SiteSettingDto ToDto(SiteSetting s) => new()
        {
            Id = s.Id,
            Key = s.Key,
            ValueAr = s.ValueAr,
            ValueEn = s.ValueEn
        };
    }
}
