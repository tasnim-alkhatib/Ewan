using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.offers;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class OfferService : IOfferService
    {
        private readonly EwanDbContext _db;

        public OfferService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<List<OfferDto>> GetActiveAsync()
        {
            var now = DateTime.UtcNow;

            return await _db.Offers
                .Include(o => o.PricingTiers)
                .Where(o => o.IsActive
                            && (o.StartDate == null || o.StartDate <= now)
                            && (o.EndDate == null || o.EndDate >= now))
                .OrderBy(o => o.SortOrder)
                .Select(o => ToDto(o))
                .ToListAsync();
        }

        public async Task<PagedResult<OfferDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.Offers.Include(o => o.PricingTiers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(o => o.TitleAr.Contains(request.Search) || o.TitleEn.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(o => o.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => ToDto(o))
                .ToListAsync();

            return new PagedResult<OfferDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<OfferDto?> GetByIdAsync(int id)
        {
            var offer = await _db.Offers.Include(o => o.PricingTiers).FirstOrDefaultAsync(o => o.Id == id);
            return offer is null ? null : ToDto(offer);
        }

        public async Task<OfferDto> CreateAsync(UpsertOfferRequest request, int adminUserId)
        {
            var offer = new Offer
            {
                TitleAr = request.TitleAr,
                TitleEn = request.TitleEn,
                DescriptionAr = request.DescriptionAr,
                DescriptionEn = request.DescriptionEn,
                ImageUrl = request.ImageUrl,
                WhatsAppLink = request.WhatsAppLink,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedByUserId = adminUserId,
                PricingTiers = request.PricingTiers.Select(t => new OfferPricingTier
                {
                    NationalityAr = t.NationalityAr,
                    NationalityEn = t.NationalityEn,
                    DurationAr = t.DurationAr,
                    DurationEn = t.DurationEn,
                    Price = t.Price,
                    RenewalPrice = t.RenewalPrice,
                    SortOrder = t.SortOrder,
                    CreatedByUserId = adminUserId
                }).ToList()
            };

            _db.Offers.Add(offer);
            await _db.SaveChangesAsync();
            return ToDto(offer);
        }

        public async Task<OfferDto?> UpdateAsync(int id, UpsertOfferRequest request, int adminUserId)
        {
            var offer = await _db.Offers.Include(o => o.PricingTiers).FirstOrDefaultAsync(o => o.Id == id);
            if (offer is null) return null;

            offer.TitleAr = request.TitleAr;
            offer.TitleEn = request.TitleEn;
            offer.DescriptionAr = request.DescriptionAr;
            offer.DescriptionEn = request.DescriptionEn;
            offer.ImageUrl = request.ImageUrl;
            offer.WhatsAppLink = request.WhatsAppLink;
            offer.StartDate = request.StartDate;
            offer.EndDate = request.EndDate;
            offer.SortOrder = request.SortOrder;
            offer.IsActive = request.IsActive;
            offer.UpdatedByUserId = adminUserId;

            // مقارنة صفوف الأسعار الجاية من الفرونت بالموجود فعليًا في قاعدة البيانات:
            // اللي مبعوتلوش Id = صف جديد نضيفه
            // اللي معاه Id وموجود = نعدّل عليه
            // اللي موجود في قاعدة البيانات ومبقاش موجود في الطلب = اتشال، نحذفه
            var incomingIds = request.PricingTiers.Where(t => t.Id.HasValue).Select(t => t.Id!.Value).ToHashSet();
            var tiersToRemove = offer.PricingTiers.Where(t => !incomingIds.Contains(t.Id)).ToList();
            foreach (var tier in tiersToRemove)
            {
                _db.OfferPricingTiers.Remove(tier);
            }

            foreach (var tierRequest in request.PricingTiers)
            {
                if (tierRequest.Id.HasValue)
                {
                    var existingTier = offer.PricingTiers.FirstOrDefault(t => t.Id == tierRequest.Id.Value);
                    if (existingTier is null) continue;

                    existingTier.NationalityAr = tierRequest.NationalityAr;
                    existingTier.NationalityEn = tierRequest.NationalityEn;
                    existingTier.DurationAr = tierRequest.DurationAr;
                    existingTier.DurationEn = tierRequest.DurationEn;
                    existingTier.Price = tierRequest.Price;
                    existingTier.RenewalPrice = tierRequest.RenewalPrice;
                    existingTier.SortOrder = tierRequest.SortOrder;
                    existingTier.UpdatedByUserId = adminUserId;
                }
                else
                {
                    offer.PricingTiers.Add(new OfferPricingTier
                    {
                        NationalityAr = tierRequest.NationalityAr,
                        NationalityEn = tierRequest.NationalityEn,
                        DurationAr = tierRequest.DurationAr,
                        DurationEn = tierRequest.DurationEn,
                        Price = tierRequest.Price,
                        RenewalPrice = tierRequest.RenewalPrice,
                        SortOrder = tierRequest.SortOrder,
                        CreatedByUserId = adminUserId
                    });
                }
            }

            await _db.SaveChangesAsync();
            return ToDto(offer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var offer = await _db.Offers.FindAsync(id);
            if (offer is null) return false;

            offer.IsDeleted = true; // Soft delete - صفوف الأسعار بتفضل موجودة مرتبطة بيه، مش بتتحذف
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder)
        {
            var ids = idToSortOrder.Keys.ToList();
            var offers = await _db.Offers.Where(o => ids.Contains(o.Id)).ToListAsync();

            foreach (var offer in offers)
            {
                offer.SortOrder = idToSortOrder[offer.Id];
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private static OfferDto ToDto(Offer o) => new()
        {
            Id = o.Id,
            TitleAr = o.TitleAr,
            TitleEn = o.TitleEn,
            DescriptionAr = o.DescriptionAr,
            DescriptionEn = o.DescriptionEn,
            ImageUrl = o.ImageUrl,
            WhatsAppLink = o.WhatsAppLink,
            StartDate = o.StartDate,
            EndDate = o.EndDate,
            SortOrder = o.SortOrder,
            IsActive = o.IsActive,
            PricingTiers = o.PricingTiers
                .OrderBy(t => t.SortOrder)
                .Select(t => new OfferPricingTierDto
                {
                    Id = t.Id,
                    NationalityAr = t.NationalityAr,
                    NationalityEn = t.NationalityEn,
                    DurationAr = t.DurationAr,
                    DurationEn = t.DurationEn,
                    Price = t.Price,
                    RenewalPrice = t.RenewalPrice,
                    SortOrder = t.SortOrder
                }).ToList()
        };
    }
}
