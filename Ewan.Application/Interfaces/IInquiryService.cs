using Ewan.Application.DTOs.Common;
using Ewan.Application.DTOs.Inquiries;
using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.Interfaces
{
    public interface IInquiryService
    {
        // بيستخدمه الموقع العام - أي حد يقدر يبعت استفسار من غير تسجيل دخول
        Task<InquiryDto> CreateAsync(CreateInquiryRequest request);

        // بيستخدمهم لوحة التحكم فقط
        Task<PagedResult<InquiryDto>> GetAllAsync(PagedRequest request, InquiryStatus? status, Sector? sector);
        Task<InquiryDto?> GetByIdAsync(int id);
        Task<InquiryDto?> UpdateStatusAsync(int id, UpdateInquiryStatusRequest request, int adminUserId);
        Task<bool> DeleteAsync(int id);

        // عدد الاستفسارات الجديدة - بيستخدمه اللوحة لعرض Badge على الأيقونة (Polling كل فترة)
        Task<int> GetNewCountAsync();
    }
}
