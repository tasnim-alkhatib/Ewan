using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Faqs;
using Ewan.Application.Interfaces;
using Ewan.Domain.Entities;
using Ewan.Domain.Enums;
using Ewan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ewan.Infrastructure.Services
{
    public class FaqService : IFaqService
    {
        private readonly EwanDbContext _db;

        public FaqService(EwanDbContext db)
        {
            _db = db;
        }

        public async Task<List<FaqDto>> GetBySectorAsync(Sector sector)
        {
            return await _db.Faqs
                .Where(f => f.IsActive && f.Sector == sector)
                .OrderBy(f => f.SortOrder)
                .Select(f => ToDto(f))
                .ToListAsync();
        }

        public async Task<PagedResult<FaqDto>> GetAllAsync(PagedRequest request)
        {
            var query = _db.Faqs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(f => f.QuestionAr.Contains(request.Search) || f.QuestionEn.Contains(request.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.SortOrder)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => ToDto(f))
                .ToListAsync();

            return new PagedResult<FaqDto>
            {
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<FaqDto?> GetByIdAsync(int id)
        {
            var faq = await _db.Faqs.FindAsync(id);
            return faq is null ? null : ToDto(faq);
        }

        public async Task<FaqDto> CreateAsync(UpsertFaqRequest request, int adminUserId)
        {
            var faq = new Faq
            {
                Sector = request.Sector,
                QuestionAr = request.QuestionAr,
                QuestionEn = request.QuestionEn,
                AnswerAr = request.AnswerAr,
                AnswerEn = request.AnswerEn,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive,
                CreatedByUserId = adminUserId
            };

            _db.Faqs.Add(faq);
            await _db.SaveChangesAsync();
            return ToDto(faq);
        }

        public async Task<FaqDto?> UpdateAsync(int id, UpsertFaqRequest request, int adminUserId)
        {
            var faq = await _db.Faqs.FindAsync(id);
            if (faq is null) return null;

            faq.Sector = request.Sector;
            faq.QuestionAr = request.QuestionAr;
            faq.QuestionEn = request.QuestionEn;
            faq.AnswerAr = request.AnswerAr;
            faq.AnswerEn = request.AnswerEn;
            faq.SortOrder = request.SortOrder;
            faq.IsActive = request.IsActive;
            faq.UpdatedByUserId = adminUserId;

            await _db.SaveChangesAsync();
            return ToDto(faq);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var faq = await _db.Faqs.FindAsync(id);
            if (faq is null) return false;

            faq.IsDeleted = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReorderAsync(Dictionary<int, int> idToSortOrder)
        {
            var ids = idToSortOrder.Keys.ToList();
            var faqs = await _db.Faqs.Where(f => ids.Contains(f.Id)).ToListAsync();

            foreach (var faq in faqs)
            {
                faq.SortOrder = idToSortOrder[faq.Id];
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private static FaqDto ToDto(Faq f) => new()
        {
            Id = f.Id,
            Sector = f.Sector,
            QuestionAr = f.QuestionAr,
            QuestionEn = f.QuestionEn,
            AnswerAr = f.AnswerAr,
            AnswerEn = f.AnswerEn,
            SortOrder = f.SortOrder,
            IsActive = f.IsActive
        };
    }
}
