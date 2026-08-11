using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Locations;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class LocationService : ILocationService
    {
        private readonly EwanDbContext _db;

        public LocationService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<List<LocationDto>> GetActiveAsync()
        {
            return await _db.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.NameAr)
                .Select(l => ToDto(l))
                .ToListAsync();
        }

        public async Task<PagedResult<LocationDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.Locations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(l => l.NameAr.Contains(request.Search) || l.NameEn.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(l => l.NameAr)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(l => ToDto(l))
                .ToListAsync();

            return new PagedResult<LocationDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<LocationDto?> GetByIdAsync(int id)
        {
            var location = await _db.Locations.FindAsync(id);
            return location is null ? null : ToDto(location);
        }

        public async Task<LocationDto> CreateAsync(UpsertLocationRequest request, int adminUserId)
        {
            var location = new Location
            {
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                AddressAr = request.AddressAr,
                AddressEn = request.AddressEn,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Phone = request.Phone,
                WorkingHoursAr = request.WorkingHoursAr,
                WorkingHoursEn = request.WorkingHoursEn,
                IsActive = request.IsActive,
                CreatedByUserId = adminUserId
            };

            _db.Locations.Add(location);
            await _db.SaveChangesAsync();
            return ToDto(location);
        }

        public async Task<LocationDto?> UpdateAsync(int id, UpsertLocationRequest request, int adminUserId)
        {
            var location = await _db.Locations.FindAsync(id);
            if (location is null) return null;

            location.NameAr = request.NameAr;
            location.NameEn = request.NameEn;
            location.AddressAr = request.AddressAr;
            location.AddressEn = request.AddressEn;
            location.Latitude = request.Latitude;
            location.Longitude = request.Longitude;
            location.Phone = request.Phone;
            location.WorkingHoursAr = request.WorkingHoursAr;
            location.WorkingHoursEn = request.WorkingHoursEn;
            location.IsActive = request.IsActive;
            location.UpdatedByUserId = adminUserId;

            await _db.SaveChangesAsync();
            return ToDto(location);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _db.Locations.FindAsync(id);
            if (location is null) return false;

            location.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        private static LocationDto ToDto(Location l) => new()
        {
            Id = l.Id,
            NameAr = l.NameAr,
            NameEn = l.NameEn,
            AddressAr = l.AddressAr,
            AddressEn = l.AddressEn,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Phone = l.Phone,
            WorkingHoursAr = l.WorkingHoursAr,
            WorkingHoursEn = l.WorkingHoursEn,
            IsActive = l.IsActive
        };
    }
}
