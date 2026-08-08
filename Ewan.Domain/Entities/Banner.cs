using System;
using System.Collections.Generic;
using System.Text;
using Ewan.Domain.Common;
using Ewan.Domain.Enums;

namespace Ewan.Domain.Entities
{
    public class Banner : BaseEntity
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? SubtitleAr { get; set; }
        public string? SubtitleEn { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }         // ممكن يكون رابط داخلي أو خارجي (واتساب مثلا)
        public BannerLocation Location { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CreatedByUserId { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime? StartDate { get; set; }      // لو عايز البانر يظهر في فترة محددة فقط
        public DateTime? EndDate { get; set; }
    }
}
