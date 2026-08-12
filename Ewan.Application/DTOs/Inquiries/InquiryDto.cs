using Ewan.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Inquiries
{
    public class InquiryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }
        public Sector? Sector { get; set; }
        public InquirySource Source { get; set; }
        public InquiryStatus Status { get; set; }
        public int? ServiceItemId { get; set; }
        public string? ServiceItemNameAr { get; set; }
        public int? OfferId { get; set; }
        public string? OfferTitleAr { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ده اللي الموقع العام بيبعته لما عميل يملأ فورم استفسار - Endpoint مفتوح بدون تسجيل دخول
    public class CreateInquiryRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Message { get; set; }
        public Sector? Sector { get; set; }
        public InquirySource Source { get; set; }
        public int? ServiceItemId { get; set; }
        public int? OfferId { get; set; }
    }

    // ده اللي لوحة التحكم بتستخدمه لما فريق المبيعات يتابع الطلب (يغيّر حالته أو يضيف ملاحظة)
    public class UpdateInquiryStatusRequest
    {
        public InquiryStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
