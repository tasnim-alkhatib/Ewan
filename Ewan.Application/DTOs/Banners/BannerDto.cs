using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Domain.Enums;

namespace Ewan.Application.DTOs.Banners
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubtitleAr { get; set; }
        public string? SubtitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public BannerLocation Location { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    // نفس الشكل بيستخدم للإنشاء والتعديل، الفرق إن الـ Id بيتبعت في التعديل بس عن طريق الـ Route
    public class UpsertBannerRequest
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubtitleAr { get; set; }
        public string? SubtitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
        public BannerLocation Location { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
