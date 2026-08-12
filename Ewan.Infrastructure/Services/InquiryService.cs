using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Inquiries;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Domain.Enums;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class InquiryService : IInquiryService
    {
        private readonly EwanDbContext _db;

        public InquiryService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<InquiryDto> CreateAsync(CreateInquiryRequest request)
        {
            var inquiry = new Inquiry
            {
                FullName = request.FullName,
                Phone = request.Phone,
                Email = request.Email,
                Message = request.Message,
                Sector = request.Sector,
                Source = request.Source,
                Status = InquiryStatus.New,
                ServiceItemId = request.ServiceItemId,
                OfferId = request.OfferId
            };

            _db.Inquiries.Add(inquiry);
            await _db.SaveChangesAsync();

            // نرجّع نسخة محمّلة بالعلاقات (اسم الخدمة/العرض) عشان الـ Dto يخرج كامل
            return await GetByIdAsync(inquiry.Id) ?? ToDto(inquiry);
        }

        public async Task<PagedResult<InquiryDto>> GetAllAsync(PagedRequest request, InquiryStatus? status, Sector? sector)
        {
            var query = _db.Inquiries
                .Include(i => i.ServiceItem)
                .Include(i => i.Offer)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(i => i.Status == status.Value);

            if (sector.HasValue)
                query = query.Where(i => i.Sector == sector.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(i => i.FullName.Contains(request.Search) || i.Phone.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(i => i.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(i => ToDto(i))
                .ToListAsync();

            return new PagedResult<InquiryDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<InquiryDto?> GetByIdAsync(int id)
        {
            var inquiry = await _db.Inquiries
                .Include(i => i.ServiceItem)
                .Include(i => i.Offer)
                .FirstOrDefaultAsync(i => i.Id == id);

            return inquiry is null ? null : ToDto(inquiry);
        }

        public async Task<InquiryDto?> UpdateStatusAsync(int id, UpdateInquiryStatusRequest request, int adminUserId)
        {
            var inquiry = await _db.Inquiries
                .Include(i => i.ServiceItem)
                .Include(i => i.Offer)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inquiry is null) return null;

            inquiry.Status = request.Status;
            inquiry.Notes = request.Notes;
            inquiry.UpdatedByUserId = adminUserId;

            await _db.SaveChangesAsync();
            return ToDto(inquiry);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inquiry = await _db.Inquiries.FindAsync(id);
            if (inquiry is null) return false;

            inquiry.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetNewCountAsync()
        {
            return await _db.Inquiries.CountAsync(i => i.Status == InquiryStatus.New);
        }

        private static InquiryDto ToDto(Inquiry i) => new()
        {
            Id = i.Id,
            FullName = i.FullName,
            Phone = i.Phone,
            Email = i.Email,
            Message = i.Message,
            Sector = i.Sector,
            Source = i.Source,
            Status = i.Status,
            ServiceItemId = i.ServiceItemId,
            ServiceItemNameAr = i.ServiceItem?.NameAr,
            OfferId = i.OfferId,
            OfferTitleAr = i.Offer?.TitleAr,
            Notes = i.Notes,
            CreatedAt = i.CreatedAt
        };
    }
}
