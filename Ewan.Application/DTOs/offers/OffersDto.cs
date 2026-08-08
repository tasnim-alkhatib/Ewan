using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.offers
{
    // صف سعر واحد - نفس الشكل بيتستخدم للعرض وللإنشاء/التعديل
    public class OfferPricingTierDto
    {
        public int Id { get; set; }
        public string NationalityAr { get; set; } = string.Empty;
        public string NationalityEn { get; set; } = string.Empty;
        public string DurationAr { get; set; } = string.Empty;
        public string DurationEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? RenewalPrice { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpsertOfferPricingTierRequest
    {
        // Id بيبقى null لو الصف جديد، وبيبقى له قيمة لو التعديل خاص بصف موجود بالفعل
        public int? Id { get; set; }
        public string NationalityAr { get; set; } = string.Empty;
        public string NationalityEn { get; set; } = string.Empty;
        public string DurationAr { get; set; } = string.Empty;
        public string DurationEn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? RenewalPrice { get; set; }
        public int SortOrder { get; set; }
    }

    public class OfferDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? WhatsAppLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<OfferPricingTierDto> PricingTiers { get; set; } = new();
    }

    // عند الإنشاء أو التعديل، بتتبعت قائمة الأسعار كاملة مع العرض نفسه في نفس الطلب
    // (بدل ما تعمل زميلتك طلبات منفصلة لكل صف سعر)
    public class UpsertOfferRequest
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? WhatsAppLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public List<UpsertOfferPricingTierRequest> PricingTiers { get; set; } = new();
    }
}
